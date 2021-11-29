using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class SharkDirectCollisionSystem : ReactiveSystem<SharkCollisionComponent>
    {
        private readonly EcsFilter<PlayerComponent, ImpactComponent, LinkComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private EcsWorld _world;

        protected override bool DeleteEvent => false;

        protected override EcsFilter<SharkCollisionComponent> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {


        }
    }
}