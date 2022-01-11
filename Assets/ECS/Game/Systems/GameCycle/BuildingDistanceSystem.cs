using System;
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
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class BuildingDistanceSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsWorld _world;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<BuildingComponent, LinkComponent> _buildings;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent> _player;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private EcsEntity _buildingEntity;
        private PlayerView _playerView;
        private BuildingView _buildingView;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _playerEntity = _player.GetEntity(i);
                foreach (var j in _buildings)
                {
                    _buildingEntity = _buildings.GetEntity(j);
                    _buildingView = _buildings.Get2(j).Get<BuildingView>();
                    // No need check distance to building, if player can't spend resources here (always or anymore)
                    if (_buildingView.GetResourcesSpend() == null ||
                        !_buildingView.GetResourcesSpend().gameObject.activeSelf ||
                        !_buildingView.GetResourcesSpend().gameObject.activeInHierarchy)
                        continue;
                    if (Vector3.Distance(_buildingView.GetResourcesSpend().position, _player.Get3(i).Value) >
                        _playerView.GetInteractionDistance())
                        continue;
                    if (_playerEntity.Has<IsMovingComponent>())
                        continue;
                    if (_playerEntity.Get<ElapsedTimeComponent>().Value < _playerView.GetInteractionCooldown())
                        continue;
                    _playerEntity.Del<ElapsedTimeComponent>();
                    // Provided that one wood will be converted to one money resource.
                    if (!_buildingView.CheckCapacity())
                        continue;
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
                        case EBuildingType.RecyclingArea:
                            HandleRecyclingArea();
                            break;
                    }
                }
            }
        }

        private void HandleHouseSpend()
        {
            // Do nothing...
        }

        private void HandleLumberMillSpend()
        {
            // Do nothing...
        }

        private void HandleConcreteMixerSpend()
        {
            // Do nothing...
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

        private void HandleRecyclingArea()
        {
            foreach (EResourceType type in Enum.GetValues(typeof(EResourceType)))
                if (_playerView.RemoveResource(type, _buildingView.GetResourcesDelPos()))
                    return;
        }
    }
}