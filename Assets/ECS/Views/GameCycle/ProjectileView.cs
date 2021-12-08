using ECS.Game.Components.Flags;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class ProjectileView : LinkableView
    {
        [SerializeField] private float _speed = 10f;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<ProjectileComponent>().Speed = _speed;
        }
    }
}