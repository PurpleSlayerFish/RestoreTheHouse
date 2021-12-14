using System;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Services.ElapsedTimeService;
using Runtime.Signals;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    public class GunView : LinkableView
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private IElapsedTimeService _elapsedTimeService;
        
        [SerializeField] private TMP_Text _workshopDpsValue;
        [SerializeField] private TMP_Text _workshopHeavyStageValue;
        [SerializeField] private TMP_Text _onGunDpsValue;
        [SerializeField] private Transform _gunHandle;
        [SerializeField] private Transform _gunClip;
        [SerializeField] private Transform _rotationRoot;
        [SerializeField] private Transform _directionFrom;
        [SerializeField] private MeshRenderer _light1;
        [SerializeField] private MeshRenderer _light2;
        [SerializeField] private MeshRenderer _light3;
        [SerializeField] private Material _lightOn;
        [SerializeField] private Material _lightOff;
        [SerializeField] private ParticleSystem _poof;
        
        [SerializeField] private float _workshopProjectileDeathDistance = 10f;
        [SerializeField] private float _combatProjectileDeathDistance = 200f;
        [SerializeField] private float _heavyStageCondition = 25f;
        [SerializeField] private float _heavyStageRotationSpeed = 2f;
        [SerializeField] private float _heavyStageDampingSpeed = 3f;

        [SuppressMessage("ReSharper", "SpecifyACultureInStringConversionExplicitly")]
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<ProjectileDeathZoneComponent>().Distance = _workshopProjectileDeathDistance;
            _signalBus.GetStream<SignalUpdateDps>().Subscribe(x =>
            {
                _workshopDpsValue.text = Math.Round(x.Dps, 2).ToString();
                UpdateLights();
            }).AddTo(this);
            _workshopHeavyStageValue.text = _heavyStageCondition.ToString();
        }

        public void SetCombatProjectileDeathDistance()
        {
            Entity.Get<ProjectileDeathZoneComponent>().Distance = _combatProjectileDeathDistance;
        }

        public Vector3 GetDirection()
        {
            return Transform.position - _directionFrom.position;
        }

        public ref Transform GetRotationRoot()
        {
            return ref _rotationRoot;
        }

        public ref float GetHeavyStageCondition()
        {
            return ref _heavyStageCondition;
        }

        public void DoBarrelRotation()
        {
            _rotationRoot.Rotate(_heavyStageRotationSpeed * RotationSpeedClamp(), 0, 0);
        }

        public void StopBarrelRotation()
        {
            ref var elapsedTimeComponent = ref Entity.Get<ElapsedTimeComponent>().Value;
            elapsedTimeComponent -= _elapsedTimeService.GetElapsedTime() * _heavyStageDampingSpeed;
            elapsedTimeComponent = RotationSpeedClamp();
            _rotationRoot.Rotate(_heavyStageRotationSpeed * elapsedTimeComponent, 0, 0);
        }
        
        private float RotationSpeedClamp()
        {
            return Mathf.Clamp(Entity.Get<ElapsedTimeComponent>().Value, 0f, 4f);
        }

        private void UpdateLights()
        {
            ref var fireRate = ref Entity.Get<GunComponent>().TotalFireRate;
            if (fireRate >= _heavyStageCondition / 3)
                _light1.material = _lightOn;
            else
                _light1.material = _lightOff;
            if (fireRate >= _heavyStageCondition / 3 * 2)
                _light2.material = _lightOn;
            else
                _light2.material = _lightOff;
            if (fireRate >= _heavyStageCondition)
            {
                _light3.material = _lightOn;
                ShowHeavyStage();
            }
            else
            {
                _light3.material = _lightOff;
                HideHeavyStage();
            }
        }

        private void ShowHeavyStage()
        {
            if(_gunClip.gameObject.activeSelf)
                return;
            _gunClip.gameObject.SetActive(true);
            _gunHandle.gameObject.SetActive(true);
            _gunClip.DOScale(Vector3.one, 0.4f).SetEase(Ease.Unset);
            _gunHandle.DOScale(Vector3.one, 0.4f).SetEase(Ease.Unset);
            _poof.gameObject.SetActive(true);
        }

        private void HideHeavyStage()
        {
            if(!_gunClip.gameObject.activeSelf)
                return;
            _gunHandle.DOScale(Vector3.zero, 0.4f).SetEase(Ease.Unset);
            _gunClip.DOScale(Vector3.zero, 0.4f).SetEase(Ease.Unset).OnComplete(() =>
            {
                _gunClip.gameObject.SetActive(false);
                _gunHandle.gameObject.SetActive(false);
            });
            _poof.gameObject.SetActive(true);
        }
    }
}