using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    public class ProjectileLauncherInitSystem : ReactiveSystem<EventAddComponent<ProjectileLauncherComponent>>
    {
        protected override bool DeleteEvent => true;
        protected override EcsFilter<EventAddComponent<ProjectileLauncherComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            entity.Del<PositionComponent>();
        }
    }
}