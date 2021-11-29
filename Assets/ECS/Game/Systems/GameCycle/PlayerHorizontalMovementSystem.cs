using DataBase.Game;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.Input;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    public class PlayerHorizontalMovementSystem : ReactiveSystem<PointerDragComponent>
    {
        private readonly EcsFilter<PlayerComponent, RemapPointComponent, LinkComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        protected override EcsFilter<PointerDragComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => false;
        protected override void Execute(EcsEntity entity)
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;
            (_player.Get3(0).View as PlayerView).HandleHorizontalMovement(ref entity.Get<PointerDragComponent>().Position);
        }
    }
}