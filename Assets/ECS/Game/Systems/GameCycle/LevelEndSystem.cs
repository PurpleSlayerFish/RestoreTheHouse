using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Ui.Windows.LevelComplete;
using SimpleUi.Signals;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class LevelEndSystem : ReactiveSystem<ChangeStageComponent>
    {
        [Inject] private readonly SignalBus _signalBus;
        
#pragma warning disable 649
        private EcsFilter<TargetPositionComponent> _targeters;
        private EcsFilter<PlayerComponent, LinkComponent> _player;
        private EcsFilter<WorkerComponent, LinkComponent> _worker;
#pragma warning restore 649
        protected override EcsFilter<ChangeStageComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => false;
        private bool disable;

        protected override void Execute(EcsEntity entity)
        {
            if (disable)
                return;
            ref var gameStage = ref entity.Get<ChangeStageComponent>().Value;
            if (gameStage == EGameStage.Complete)
            {
                foreach (var i in _player)
                    _player.GetEntity(i).Del<IsMovingComponent>();
                foreach (var i in _worker)
                    _worker.GetEntity(i).Del<IsMovingComponent>();
                foreach (var i in _targeters)
                    _targeters.GetEntity(i).Del<TargetPositionComponent>();
                
                _signalBus.OpenWindow<LevelCompleteWindow>();
                disable = true;
            }
        }
    }
}