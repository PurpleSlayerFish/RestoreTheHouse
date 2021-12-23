using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using Leopotam.Ecs;
using Runtime.Signals;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class AddImpactToPlayerSystem : ReactiveSystem<AddImpactEventComponent>
    {
        [Inject] private readonly SignalBus _signalBus;
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, ImpactComponent, LinkComponent> _player;
#pragma warning restore 649

        protected override bool DeleteEvent => true;
        protected override EcsFilter<AddImpactEventComponent> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            ref var playerImpact = ref _player.Get2(0).Value;
            playerImpact += entity.Get<ImpactComponent>().Value;
            _signalBus.Fire(new SignalUpdateImpact(playerImpact));
        }
    }
}