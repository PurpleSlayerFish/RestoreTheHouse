using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class TriggersDistanceSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<DistanceTriggerComponent, LinkComponent> _triggers;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent> _player;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private EcsEntity _clueEntity;
        private PlayerView _playerView;
        private DistanceTriggerView _distanceTriggerView;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _playerEntity = _player.GetEntity(i);

                foreach (var j in _triggers)
                {
                    _clueEntity = _triggers.GetEntity(j);
                    _distanceTriggerView = _triggers.Get2(j).Get<DistanceTriggerView>();
                    if (!_distanceTriggerView.gameObject.activeSelf ||
                        !_distanceTriggerView.gameObject.activeInHierarchy)
                        continue;
                    if (Vector3.Distance(_distanceTriggerView.Transform.position, _player.Get3(i).Value) >
                        _playerView.GetInteractionDistance())
                        continue;
                    if (_distanceTriggerView.IsOnStopMoving() && _playerEntity.Has<IsMovingComponent>())
                        continue;
                    HandleClueComplete();
                }
            }
        }
        
        private void HandleClueComplete()
        {
            _distanceTriggerView.Handle();
            _clueEntity.Get<IsDelayCleanUpComponent>().Delay = 1.5f;
        }
    }
    
    public struct DistanceTriggerComponent : IEcsIgnoreInFilter
    {
        
    }
}