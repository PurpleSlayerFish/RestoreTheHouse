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
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<BallComponent, LinkComponent> _ball;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private EcsEntity _triggerEntity;
        private PlayerView _playerView;
        private BallView _ballView;
        private DistanceTriggerView _distanceTriggerView;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _ball)
                _ballView = _ball.Get2(i).Get<BallView>();
            
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _playerEntity = _player.GetEntity(i);

                foreach (var j in _triggers)
                {
                    _triggerEntity = _triggers.GetEntity(j);
                    _distanceTriggerView = _triggers.Get2(j).Get<DistanceTriggerView>();
                    
                    if (!_distanceTriggerView.gameObject.activeSelf ||
                        !_distanceTriggerView.gameObject.activeInHierarchy)
                        continue;
                    
                    if (Vector3.Distance(_playerView.Transform.position, _distanceTriggerView.Transform.position) > _distanceTriggerView.GetTriggerDistance() 
                        && Vector3.Distance(_ballView.Transform.position, _distanceTriggerView.Transform.position) > _distanceTriggerView.GetTriggerDistance())
                        continue;
                    
                    if (_distanceTriggerView.IsOnStopMoving() && _playerEntity.Has<IsMovingComponent>())
                        continue;
                    foreach (var unlockable in _distanceTriggerView.GetUnlockable())
                        unlockable?.SetActive(true);
                    foreach (var lockable in _distanceTriggerView.GetLockable())
                        lockable?.SetActive(false);
                    foreach (var linkableView in _distanceTriggerView.GetViews())
                        linkableView?.Entity.Del<EcsDisableComponent>();
                    _triggerEntity.Get<IsDelayCleanUpComponent>().Delay = 1.5f;
                }
            }
        }
    }
    
    public struct DistanceTriggerComponent : IEcsIgnoreInFilter
    {
        
    }
    
    public struct EcsDisableComponent : IEcsIgnoreInFilter
    {
    }
}