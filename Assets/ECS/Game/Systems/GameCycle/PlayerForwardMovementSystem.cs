using DataBase.Game;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class PlayerForwardMovementSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, PositionComponent, RotationComponent, SpeedComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PathPointComponent, PositionComponent, RotationComponent, UIdComponent> _pathPoints;
#pragma warning restore 649

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
                    // InitPathComplete();
                    return;
                }
                ref var targetPos = ref entity.Get<TargetPositionComponent>();
                targetPos.Value = nextPathPoint;
                targetPos.Speed = _player.Get4(i).Value;
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
                        ref var playerRot = ref _player.Get3(0).Value;
                        ref var targetRot = ref playerEntity.Get<TargetRotationComponent>();
                        ref var direction = ref _pathPoints.GetEntity(i).Get<RotationDirectionComponent>().Direction;
                        targetRot.Value = Quaternion.Euler(playerRot.eulerAngles.x + direction.x, playerRot.eulerAngles.y + direction.y, playerRot.eulerAngles.z + direction.z);
                        targetRot.Speed = _pathPoints.GetEntity(i).Get<SpeedComponent>().Value;
                    }

                    if (_pathPoints.GetEntity(i).Has<CombatPointComponent>())
                    {
                        _player.GetEntity(0).Get<InCombatComponent>().PathPoint = _pathPoints.Get4(i).Value;
                        _player.GetEntity(0).Get<EventAddComponent<InCombatComponent>>();
                    }

                    _pathPoints.GetEntity(i).Get<IsDestroyedComponent>();
                    continue;
                }
                if (!(distance < minDistance)) continue;
                minDistance = distance;
                closestPoint = pos;
            }
            return closestPoint;
        }
    }
}