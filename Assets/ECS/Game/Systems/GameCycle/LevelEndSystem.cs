using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Ui.Windows.GameOver;
using Runtime.Game.Ui.Windows.LevelComplete;
using SimpleUi.Signals;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class LevelEndSystem : ReactiveSystem<ChangeStageComponent>
    {
        [Inject] private readonly SignalBus _signalBus;
        private EcsFilter<PlayerComponent, LinkComponent> _player;
        protected override EcsFilter<ChangeStageComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => false;
        private bool disable;

        protected override void Execute(EcsEntity entity)
        {
            if (disable)
                return;
            ref var gameStage = ref entity.Get<ChangeStageComponent>().Value;
            if (gameStage == EGameStage.Lose)
            {
                _signalBus.OpenWindow<GameOverWindow>();
                (_player.Get2(0).View as PlayerView)?.InitLevelLose();
                disable = true;
            }
            if (gameStage == EGameStage.Complete)
            {
                _signalBus.OpenWindow<LevelCompleteWindow>();
                (_player.Get2(0).View as PlayerView)?.InitLevelComplete();
                disable = true;
            }
        }
    }
}