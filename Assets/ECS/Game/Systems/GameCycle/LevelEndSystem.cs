using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Ui.Windows.GameOver;
using Runtime.Game.Ui.Windows.LevelComplete;
using SimpleUi.Signals;
using Zenject;

#pragma warning disable 649

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
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
            switch (entity.Get<ChangeStageComponent>().Value)
            {
                case EGameStage.Lose:
                    HandleLevelLose();
                    disable = true;
                    break;
                case EGameStage.Complete:
                    HandleLevelComplete();
                    disable = true;
                    break;
            }
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void HandleLevelComplete()
        {
            foreach (var i in _player)
            {
                (_player.Get2(i).View as PlayerView).SetIdleAnimation();
                _player.Get2(i).View.Transform.DOMoveY(0, 1f).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => _signalBus.OpenWindow<LevelCompleteWindow>());
            }
        }
        
        private void HandleLevelLose()
        {
            foreach (var i in _player)
            {
                _player.Get2(i).View.Transform.DOMoveY(0, 1.5f).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => _signalBus.OpenWindow<GameOverWindow>());
            }
        }
    }
}