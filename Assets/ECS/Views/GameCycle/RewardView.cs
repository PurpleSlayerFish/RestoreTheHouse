using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class RewardView : LinkableView
    {
        [SerializeField] private int _rewardValue = 1;
        [SerializeField] private float _movementSpeed = 10f;
        [SerializeField] private float _distanceToGet = 2f;
        [SerializeField] private ParticleSystem _spawnEffect;

        public override void Link(EcsEntity entity)
        {
            entity.Get<ImpactComponent>().Value = _rewardValue;
            entity.Get<SpeedComponent<PositionComponent>>().Value = _movementSpeed;
            _spawnEffect.gameObject.SetActive(true);
        }

        public ref float GetDistanceToGet()
        {
            return ref _distanceToGet;
        }
    }
}