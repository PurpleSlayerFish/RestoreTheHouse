using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    public class PiranhaAttachSystem : ReactiveSystem<EventAddComponent<PiranhaComponent>>
    {
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        protected override EcsFilter<EventAddComponent<PiranhaComponent>> ReactiveFilter { get; }
        protected override void Execute(EcsEntity entity)
        {
            (_player.Get2(0).View as PlayerView).AttachPiranha(entity.Get<LinkComponent>().View as PiranhaView);
        }
    }
}