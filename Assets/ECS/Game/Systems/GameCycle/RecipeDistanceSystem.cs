using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Signals;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    public class RecipeDistanceSystem : IEcsUpdateSystem
    {
        [Inject] private SignalBus _signalBus;
        [Inject] private ScreenVariables _screenVariables;

#pragma warning disable 649
        private readonly EcsWorld _world;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<RecipeComponent, LinkComponent> _recipes;
        private readonly EcsFilter<BuildingComponent, ResourceProductionComponent> _buildings;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent> _player;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private EcsEntity _recipeEntity;
        private PlayerView _playerView;
        private RecipeView _recipeView;

        private readonly string ConcreteMixerForEachUpgradeProduction = "ConcreteMixerForEachUpgradeProduction";
        private readonly string LumberMillForEachUpgradeProduction = "LumberMillForEachUpgradeProduction";
        private readonly string LumberMillWorkerProduction = "LumberMillWorkerProduction";

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _playerEntity = _player.GetEntity(i);

                foreach (var j in _recipes)
                {
                    _recipeEntity = _recipes.GetEntity(j);
                    _recipeView = _recipes.Get2(j).Get<RecipeView>();
                    if (_recipeView.GetResourcesSpend() == null ||
                        !_recipeView.GetResourcesSpend().gameObject.activeSelf ||
                        !_recipeView.GetResourcesSpend().gameObject.activeInHierarchy)
                        continue;
                    if (Vector3.Distance(_recipeView.GetResourcesSpend().position, _player.Get3(i).Value) >
                        _playerView.GetInteractionDistance())
                        continue;
                    if (_playerEntity.Has<IsMovingComponent>())
                        continue;
                    if (_playerEntity.Get<ElapsedTimeComponent>().Value < _playerView.GetInteractionCooldown())
                        continue;
                    _playerEntity.Del<ElapsedTimeComponent>();
                    HandleRecipeProgress();
                }
            }
        }

        private void HandleRecipeProgress()
        {
            ref var recipeResources = ref _recipeView.GetResources();
            for (int i = 0; i < recipeResources.Length; i++)
            {
                if (_recipeView.GetResourcesCount()[i] <= 0)
                    continue;
                if (_playerView.RemoveResource(recipeResources[i], _recipeView.GetResourcesDelPos()))
                {
                    _recipeView.GetResourcesCount()[i]--;
                    _signalBus.Fire(new SignalRecipeUpdate(recipeResources[i],
                        _recipeEntity.Get<UIdComponent>().Value));
                    break;
                }
            }

            if (_recipeView.IsCompleted())
            {
                _recipeView.HandleComplete();
                _recipeEntity.Get<IsDelayDestroyedComponent>().Delay = 1.5f;
                if (!_playerView.GetNavMeshAgent()
                    .CalculatePath(_playerEntity.Get<PositionComponent>().Value, new NavMeshPath()))
                    _playerEntity.Get<PositionComponent>().Value = _recipeView.GetResourcesSpend().position;
                HandleCompletedRecipeType();
            }
        }

        private void HandleCompletedRecipeType()
        {
            switch (_recipeEntity.Get<RecipeComponent>().Type)
            {
                case ERecipeType.Finish:
                    _world.SetStage(EGameStage.Complete);
                    break;
                case ERecipeType.ConcreteMixerUpgrade:
                    foreach (var i in _buildings)
                        if (_buildings.Get1(i).Type == EBuildingType.ConcreteMixer)
                        {
                            _buildings.Get2(i).Value +=
                                _screenVariables.GetFloatValue(ConcreteMixerForEachUpgradeProduction);
                            break;
                        }

                    break;
                case ERecipeType.LumperMillUpgrade:
                    foreach (var i in _buildings)
                        if (_buildings.Get1(i).Type == EBuildingType.LumberMill)
                        {
                            _buildings.Get2(i).Value +=
                                _screenVariables.GetFloatValue(LumberMillForEachUpgradeProduction);
                            break;
                        }

                    break;
                case ERecipeType.LumberMillWorkerEmploy:
                    foreach (var i in _buildings)
                        if (_buildings.Get1(i).Type == EBuildingType.LumberMill)
                        {
                            _buildings.Get2(i).Value +=
                                _screenVariables.GetFloatValue(LumberMillWorkerProduction);
                            _world.CreateWorker();
                            break;
                        }

                    break;
            }
        }
    }
}