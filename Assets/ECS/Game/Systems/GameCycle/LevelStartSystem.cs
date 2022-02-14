using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Ui;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.AnalyticsService;
using SimpleUi.Signals;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;
// ReSharper disable All
#pragma warning disable 649

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class LevelStartSystem : ReactiveSystem<LevelStartEventComponent>
    {
        [Inject] private IAnalyticsService _analyticsService;
        [Inject] private SignalBus _signalBus;
        [Inject] private ScreenVariables _screenVariables;
        private const string PLAYER_START = "PlayerStart";
        
        private readonly EcsWorld _world;
        
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private bool started = false;

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        protected override EcsFilter<LevelStartEventComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => true;
        protected override void Execute(EcsEntity entity)
        {
            if (started)
                return;

            _world.SetStage(EGameStage.Play);
            _signalBus.OpenWindow<GameHudWindow>();
            _analyticsService.SendRequest("level_start");
            _world.CreateCamera();
            InitLevelData(ref entity);
            started = true;
        }

        private void InitLevelData(ref EcsEntity entity)
        {
            entity.Get<PositionComponent>().Value = _screenVariables.GetTransformPoint(PLAYER_START).position;
            entity.Get<RotationComponent>().Value = _screenVariables.GetTransformPoint(PLAYER_START).rotation;
        }
    }

    public struct LevelStartEventComponent : IEcsIgnoreInFilter
    {
    }
}