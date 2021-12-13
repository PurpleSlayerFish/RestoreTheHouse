using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class ProjectileLauncherInitSystem : ReactiveSystem<EventAddComponent<ProjectileLauncherComponent>>
    {
#pragma warning disable 649
        private EcsFilter<GunComponent, LinkComponent> _gun;
#pragma warning restore 649
        
        protected override bool DeleteEvent => true;
        protected override EcsFilter<EventAddComponent<ProjectileLauncherComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            foreach (var i in _gun)
                entity.Get<LinkComponent>().View.Transform.SetParent((_gun.Get2(i).View as GunView).GetRotationRoot());
            entity.Del<PositionComponent>();
        }
    }
}