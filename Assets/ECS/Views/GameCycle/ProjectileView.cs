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
        [SerializeField] private float _speed = 10f;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);

            if (entity.Has<InWorkshopComponent>())
                entity.Get<SpeedComponent>().Value = _speed / 3;
            else
                entity.Get<SpeedComponent>().Value = _speed;
        }

        public void Impact()
        {
            Entity.Get<IsDestroyedComponent>();
        }

        public ref Collider GetCollider()
        {
            return ref _collider;
        }
    }
}