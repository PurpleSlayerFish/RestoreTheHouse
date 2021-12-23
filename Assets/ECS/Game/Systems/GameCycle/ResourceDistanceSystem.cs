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
    public class ResourceDistanceSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<ResourceComponent, LinkComponent> _resources;
        private readonly EcsFilter<MarketComponent, LinkComponent> _markets;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent> _player;
#pragma warning restore 649

        private EcsEntity _resource;
        private MarketView _marketView;
        private PlayerView _playerView;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;


            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();

                foreach (var j in _resources)
                {
                    _resource = _resources.GetEntity(j);
                    if (_resource.Has<PickedComponent>())
                        continue;
                    if (Vector3.Distance(_resources.Get2(j).View.Transform.position, _player.Get3(i).Value) <
                        _playerView.GetInteractionDistance()
                        && _playerView.GetResourcesCount() < _playerView.GetResourcesCapacity())
                        _resource.GetAndFire<PickedComponent>();
                }

                foreach (var j in _markets)
                {
                    _marketView = _markets.Get2(j).Get<MarketView>();
                    if (Vector3.Distance(_marketView.GetSalePointPos(), _player.Get3(i).Value) <
                        _playerView.GetInteractionDistance()
                        && !_player.GetEntity(i).Has<IsMovingComponent>())
                        _playerView.RemoveResource(EResourceType.Wood, _marketView.GetClearPointPos());
                }
            }
        }
    }
}