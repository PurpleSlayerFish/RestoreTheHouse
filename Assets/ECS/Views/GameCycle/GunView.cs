using System;
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
        [SerializeField] private TMP_Text _onGunDpsValue;
        [SerializeField] private Transform _gunHandle;
        [SerializeField] private Transform _gunClip;
        [SerializeField] private Transform _rotationRoot;
        [SerializeField] private Transform _directionFrom;
        
        [SerializeField] private float _workshopProjectileDeathDistance = 10f;
        [SerializeField] private float _combatProjectileDeathDistance = 200f;
        [SerializeField] private float _heavyStageCondition = 25f;
        [SerializeField] private float _heavyStageRotationSpeed = 2f;
        [SerializeField] private float _heavyStageDampingSpeed = 3f;

        private Sequence _rotationSeq;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<ProjectileDeathZoneComponent>().Distance = _workshopProjectileDeathDistance;
            _signalBus.GetStream<SignalUpdateDps>().Subscribe(x => _workshopDpsValue.text = Math.Round(x.Dps, 1).ToString()).AddTo(this);
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

        public void SetHeavyStage()
        {
            _gunHandle.gameObject.SetActive(true);
            _gunClip.gameObject.SetActive(true);
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

        
        
    }
}