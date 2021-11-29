using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.Input;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class RemapOnAddSystem : ReactiveSystem<EventAddComponent<RemapPointComponent>>
    {
        protected override EcsFilter<EventAddComponent<RemapPointComponent>> ReactiveFilter { get; }
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        protected override void Execute(EcsEntity entity)
        {
            var playerView = _player.Get2(0).View as PlayerView;
            entity.Get<RemapPointComponent>().ModelPos = playerView.RootTransform.localPosition;
        }
    }
}