using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using Leopotam.Ecs;

namespace ECS.Game.Systems.Linked
{
    public class CleanUpSystem : ReactiveSystem<IsDestroyedComponent>
    {
        protected override EcsFilter<IsDestroyedComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => false;
        protected override void Execute(EcsEntity entity)
        {
            if (entity.Has<LinkComponent>())
                entity.Get<LinkComponent>().View.DestroyObject();
            entity.Destroy();
        }
    }
}