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
using Runtime.Signals;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    public class PlayerDistanceSystem : IEcsUpdateSystem
    {
        [Inject] private SignalBus _signalBus;
        
#pragma warning disable 649
        private readonly EcsWorld _world;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<ResourceComponent, LinkComponent> _resources;
        private readonly EcsFilter<BuildingComponent, LinkComponent> _buildings;
        private readonly EcsFilter<RecipeComponent, LinkComponent> _recipes;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent> _player;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private EcsEntity _resourceEntity;
        private EcsEntity _buildingEntity;
        private EcsEntity _recipeEntity;
        private PlayerView _playerView;
        private BuildingView _buildingView;
        private RecipeView _recipeView;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _playerEntity = _player.GetEntity(i);

                foreach (var j in _resources)
                {
                    _resourceEntity = _resources.GetEntity(j);
                    if (_resourceEntity.Has<PickedComponent>())
                        continue;
                    if (Vector3.Distance(_resources.Get2(j).View.Transform.position, _player.Get3(i).Value) <
                        _playerView.GetInteractionDistance()
                        && _playerView.GetResourcesCount() < _playerView.GetResourcesCapacity())
                        _resourceEntity.GetAndFire<PickedComponent>();
                }

                foreach (var j in _buildings)
                {
                    _buildingEntity = _buildings.GetEntity(j);
                    _buildingView = _buildings.Get2(j).Get<BuildingView>();
                    if (!_buildingView.gameObject.activeInHierarchy)
                        continue;
                    // No need check distance to building, if player can't spend resources here (always or anymore)
                    if (_buildingView.GetResourcesSpend() == null &&
                        _buildingView.GetResourcesSpend().gameObject.activeSelf == false)
                        continue;
                    if (Vector3.Distance(_buildingView.GetResourcesSpend().position, _player.Get3(i).Value) >
                        _playerView.GetInteractionDistance())
                        continue;
                    if (_playerEntity.Has<IsMovingComponent>())
                        continue;
                    if (_playerEntity.Get<ElapsedTimeComponent>().Value < _playerView.GetInteractionCooldown())
                        continue;
                    // Provided that one wood will be converted to one money resource.
                    if (!_buildingView.CheckCapacity())
                        continue;
                    _playerEntity.Del<ElapsedTimeComponent>();
                    switch (_buildings.Get1(j).Type)
                    {
                        case EBuildingType.House:
                            HandleHouseSpend();
                            break;
                        case EBuildingType.LumberMill:
                            HandleLumberMillSpend();
                            break;
                        case EBuildingType.ConcreteMixer:
                            HandleConcreteMixerSpend();
                            break;
                        case EBuildingType.TimberSaleVan:
                            HandleTimberSaleVanSpend();
                            break;
                    }
                }

                foreach (var j in _recipes)
                {
                    _recipeEntity = _recipes.GetEntity(j);
                    _recipeView = _recipes.Get2(j).Get<RecipeView>();
                    if (_recipeView.GetResourcesSpend() == null &&
                        _recipeView.GetResourcesSpend().gameObject.activeSelf == false)
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

        private void HandleHouseSpend()
        {
        }

        private void HandleLumberMillSpend()
        {
        }

        private void HandleConcreteMixerSpend()
        {
        }

        private void HandleTimberSaleVanSpend()
        {
            if (!_playerView.RemoveResource(EResourceType.Wood, _buildingView.GetResourcesDelPos()))
                return;
            var money = _world.CreateResources(EResourceType.Money);
            _buildingView.AddResources(ref money);
            money.Get<UidLinkComponent>().Link = _buildingEntity.Get<UIdComponent>().Value;
            money.Get<MoveTweenEventComponent>().EventType = ETweenEventType.ResourceDelivery;
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
                    _signalBus.Fire(new SignalResourceDeliver(recipeResources[i], _recipeEntity.Get<UIdComponent>().Value));
                    break;
                }
            }

            if (_recipeView.IsCompleted())
            {
                _recipeView.Invoke();
                _recipeEntity.Get<IsDestroyedComponent>();
                if (!_playerView.GetNavMeshAgent().CalculatePath(_playerEntity.Get<PositionComponent>().Value, new NavMeshPath()))
                    _playerEntity.Get<PositionComponent>().Value = _recipeView.GetResourcesSpend().position;
                if (_recipeView.IsFinishLevel())
                {
                    _world.SetStage(EGameStage.Complete);
                }
            }
        }
    }
}