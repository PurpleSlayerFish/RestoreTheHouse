using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Utils.Impls;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class PlayerForwardMovementSystem : IEcsUpdateSystem
    {
        private readonly EcsFilter<PlayerComponent, PositionComponent, RotationComponent, LinkComponent, ImpactComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PathPointComponent, PositionComponent, RotationComponent> _pathPoints;
        public void Run()
        {
            if(_gameStage.Get1(0).Value != EGameStage.Play) return;


            foreach (var i in _player)
            {
                var entity = _player.GetEntity(i);
                if (entity.Has<TargetPositionComponent>())
                    return;
                ref var playerPos = ref  _player.Get2(i).Value;
            
                var nextPathPoint = GetNextPathPoint(playerPos);
                if (nextPathPoint == Vector3.zero)
                {
                    InitPathComplete();
                    return;
                }
            
                ref var targetPos = ref entity.Get<TargetPositionComponent>();
                targetPos.Value = nextPathPoint;
                targetPos.Speed = (_player.Get4(i).View as PlayerView).GetCurrentSpeed(); 
            }
        }

        private Vector3 GetNextPathPoint(Vector3 playerPos)
        {
            var closestPoint = Vector3.zero;
            var minDistance = float.MaxValue;
            foreach (var i in _pathPoints)
            {
                var pos = _pathPoints.Get2(i).Value;
                var distance = Vector3.Distance(pos, playerPos);
                if (distance < 0.01f)
                {
                    var playerEntity = _player.GetEntity(0);
                    if (_pathPoints.GetEntity(i).Has<RotationDirectionComponent>())
                    {
                        var playerRot = _player.Get3(0).Value;
                        ref var targetRot = ref playerEntity.Get<TargetRotationComponent>();
                        var yaw = _pathPoints.GetEntity(i).Get<RotationDirectionComponent>().Value switch {
                            ERotateDirection.Left => -90,
                            ERotateDirection.Right => 90,
                            _ => 0
                        };
                        targetRot.Value = Quaternion.Euler(playerRot.eulerAngles.x, Mathf.RoundToInt(playerRot.eulerAngles.y + yaw), playerRot.eulerAngles.z);
                        targetRot.Speed = 360  * 0.15f;
                    }
                    _pathPoints.GetEntity(i).Destroy();
                    continue;
                }
                if (!(distance < minDistance)) continue;
                minDistance = distance;
                closestPoint = pos;

            }
            return closestPoint;
        }
        
        
        public void InitPathComplete()
        {
            if (_player.Get5(0).Value < 0)
                _gameStage.GetEntity(0).Get<ChangeStageComponent>().Value = EGameStage.Lose;
            else
                _gameStage.GetEntity(0).Get<ChangeStageComponent>().Value = EGameStage.Complete;
            (_player.Get4(0).View as PlayerView).IsPathComplete = true;
        }

    }
}