using DG.Tweening;
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
        private EcsEntity _resourceEntity;
        private Vector3 resourcesRotation = new Vector3(0, 90, 0);

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _resources)
            {
                _resourceView = _resources.Get2(i).Get<ResourceView>();
                _resourceEntity = _resources.GetEntity(i);
                if (HandleResourcePickup())
                    continue;
                if (HandleResourceSpend())
                    continue;
                if (HandleResourceDelivery())
                    // ReSharper disable once RedundantJumpStatement
                    continue;
            }
        }

        private bool HandleResourcePickup()
        {
            if (ETweenEventType.ResourcePickUp !=
                _resourceEntity.Get<MoveTweenEventComponent>().EventType)
                return false;
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _resourceView.Transform.DOKill();
                _resourceView.Transform.SetParent(_playerView.GetResourcesStack());
                _resourceView.Transform
                    .DOLocalMove(_resourceEntity.Get<Vector3Component<MoveTweenEventComponent>>().Value,
                        _playerView.GetInteractionDuration())
                    .SetEase(Ease.Linear);
                _resourceView.Transform.DOLocalRotate(resourcesRotation, _playerView.GetInteractionDuration())
                    .SetEase(Ease.Linear);
                _resourceEntity.Del<MoveTweenEventComponent>();
                _resourceEntity.Del<Vector3Component<MoveTweenEventComponent>>();
                return true;
            }

            return false;
        }

        private bool HandleResourceSpend()
        {
            if (ETweenEventType.ResourceSpend !=
                _resourceEntity.Get<MoveTweenEventComponent>().EventType)
                return false;
            if (!_resourceEntity.Has<PickedComponent>())
                return false;
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                var resTransform = _resourceEntity.Get<LinkComponent>().View.Transform;
                _resourceView.Transform.DOKill();
                resTransform.SetParent(null);
                resTransform.DOLocalMove(_resourceEntity.Get<Vector3Component<MoveTweenEventComponent>>().Value,
                    _playerView.GetInteractionDuration()).SetEase(Ease.Linear);
                _resourceEntity.Del<MoveTweenEventComponent>();
                _resourceEntity.Get<IsDelayCleanUpComponent>().Delay = _playerView.GetInteractionDuration() + 0.2f;
                return true;
            }

            return false;
        }

        private bool HandleResourceDelivery()
        {
            if (ETweenEventType.ResourceDelivery !=
                _resourceEntity.Get<MoveTweenEventComponent>().EventType)
                return false;
            foreach (var i in _buildings)
            {
                if (_buildings.Get3(i).Value == _resourceEntity.Get<UidLinkComponent>().Link)
                {
                    _buildingView = _buildings.Get2(i).Get<BuildingView>();
                    _resourceView.Transform.DOKill();
                    _resourceView.Transform.SetParent(_buildingView.Transform);
                    _resourceView.Transform.position = _buildingView.GetResourcesDeliveryStartPoint();
                    _resourceView.Transform.DOLocalMove(_buildingView.GetResourcesDeliveryEndPoint(),
                        _buildingView.GetResourcesDeliveryDuration()).SetEase(Ease.Linear);
                    _resourceView.Transform.DOLocalRotate(_buildingView.GetResourcesDeliveryRotation(),
                            _buildingView.GetResourcesDeliveryDuration())
                        .SetEase(Ease.Linear);
                    _resourceEntity.Del<MoveTweenEventComponent>();
                    return true;
                }
            }

            return false;
        }
    }
}