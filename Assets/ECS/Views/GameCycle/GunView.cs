using System;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
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
        [SerializeField] private TMP_Text _workshopDpsValue;
        [SerializeField] private TMP_Text _onGunDpsValue;
        [SerializeField] private Transform _gunHandle;
        [SerializeField] private Transform _gunClip;
        [SerializeField] private Transform _directionFrom;
        
        [SerializeField] private float _workshopProjectileDeathDistance = 10f;
        [SerializeField] private float _combatProjectileDeathDistance = 200f;

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
    }
}