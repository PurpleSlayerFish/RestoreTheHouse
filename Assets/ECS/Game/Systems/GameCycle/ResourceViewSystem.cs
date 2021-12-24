﻿using DG.Tweening;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class ResourceViewSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<BuildingComponent, LinkComponent, UIdComponent> _buildings;
        private readonly EcsFilter<ResourceComponent, LinkComponent, MoveTweenEventComponent> _resources;
#pragma warning restore 649

        private PlayerView _playerView;
        private ResourceView _resourceView;
        private BuildingView _buildingView;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _resources)
            {
                _resourceView = _resources.Get2(i).Get<ResourceView>();
                if (HandleResourcePickup(ref _resources.GetEntity(i)))
                    continue;
                if (HandleResourceSpend(ref _resources.GetEntity(i)))
                    continue;
                if (HandleResourceDelivery(ref _resources.GetEntity(i)))
                    continue;
            }
        }

        private bool HandleResourcePickup(ref EcsEntity resource)
        {
            if (ETweenEventType.ResourcePickUp !=
                resource.Get<MoveTweenEventComponent>().EventType)
                return false;
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _resourceView.Transform.SetParent(_playerView.GetResourcesStack());
                _resourceView.Transform
                    .DOLocalMove(resource.Get<Vector3Component<MoveTweenEventComponent>>().Value, _playerView.GetInteractionDuration())
                    .SetEase(Ease.Unset);
                _resourceView.Transform.DOLocalRotate(Vector3.zero, _playerView.GetInteractionDuration())
                    .SetEase(Ease.Unset);
                resource.Del<MoveTweenEventComponent>();
                resource.Del<Vector3Component<MoveTweenEventComponent>>();
                return true;
            }

            return false;
        }

        private bool HandleResourceSpend(ref EcsEntity resource)
        {
            if (ETweenEventType.ResourceSpend !=
                resource.Get<MoveTweenEventComponent>().EventType)
                return false;
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                var resTransform = resource.Get<LinkComponent>().View.Transform;
                resTransform.SetParent(null);
                var entity1 = resource;
                resTransform.DOLocalMove(resource.Get<Vector3Component<MoveTweenEventComponent>>().Value, _playerView.GetInteractionDuration()).SetEase(Ease.Unset)
                    .OnComplete(() => entity1.Get<IsDestroyedComponent>());
                return true;
            }
            return false;
        }

        private bool HandleResourceDelivery(ref EcsEntity resource)
        {
            if (ETweenEventType.ResourceDelivery !=
                resource.Get<MoveTweenEventComponent>().EventType)
                return false;
            foreach (var i in _buildings)
            {
                if (_buildings.Get3(i).Value == resource.Get<UidLinkComponent>().Link)
                {
                    _buildingView = _buildings.Get2(i).Get<BuildingView>();
                    _resourceView.Transform.position = _buildingView.GetResourcesDeliveryStartPoint();
                    _resourceView.Transform.DOLocalMove(_buildingView.GetResourcesDeliveryEndPoint(), _buildingView.GetResourcesDeliveryDuration()).SetEase(Ease.Unset);
                    resource.Del<MoveTweenEventComponent>();
                    resource.Del<UidLinkComponent>();
                    return true;
                }
            }
            return false;
        }
    }
}