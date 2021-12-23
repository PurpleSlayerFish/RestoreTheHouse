using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class ResourceView : LinkableView
    {
        [SerializeField] private EResourceType _type;
        
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<ResourceComponent>().Type = _type;
        }
    }
}