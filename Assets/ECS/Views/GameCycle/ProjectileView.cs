using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class ProjectileView : LinkableView
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _impactEffect;
        [SerializeField] private MeshRenderer _renderer;
        [SerializeField] private float _speed = 10f;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);

            if (entity.Has<InWorkshopComponent>())
                entity.Get<SpeedComponent<PositionComponent>>().Value = _speed / 3;
            else
                entity.Get<SpeedComponent<PositionComponent>>().Value = _speed;
        }

        public void Impact()
        {
            _impactEffect.gameObject.SetActive(true);
            _collider.enabled = false;
            _renderer.enabled = false;
            Transform.DOMove(Vector3.zero, 1f).SetRelative(true).OnComplete(() => Entity.Get<IsDestroyedComponent>());
            // Entity.Get<IsDestroyedComponent>();
        }

        public ref Collider GetCollider()
        {
            return ref _collider;
        }
    }
}