using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Services.ElapsedTimeService;
using Runtime.Services.ElapsedTimeService.Impls;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    public class MoveRotateToTargetSystem : IEcsUpdateSystem
    {
        [Inject] private IElapsedTimeService _elapsedTimeService;
#pragma warning disable 649
        private readonly EcsFilter<PositionComponent, TargetPositionComponent, SpeedComponent<PositionComponent>> _position;
        private readonly EcsFilter<RotationComponent, TargetRotationComponent> _rotation;
        private readonly EcsFilter<GameStageComponent> _gameStage;
#pragma warning restore 649
        public void Run()
        {
            if(_gameStage.Get1(0).Value != EGameStage.Play) return;
            
            foreach (var i in _position)
            {
                var speed = _position.Get3(i).Value;
                var target = _position.Get2(i).Value;
                ref var pos = ref _position.Get1(i).Value;
                pos = Vector3.MoveTowards(pos, target, Time.deltaTime * speed);
                if (Vector3.Distance(pos, target) < 0.01f)
                {
                    pos = target;
                    _position.GetEntity(i).DelAndFire<TargetPositionComponent>();
                }
            }
            
            foreach (var i in _rotation)
            {
                var speed = _rotation.Get2(i).Speed;
                var target = _rotation.Get2(i).Value;
                ref var rot = ref _rotation.Get1(i).Value;
                rot = Quaternion.RotateTowards(rot, target, _elapsedTimeService.GetElapsedTime() * speed);
                if (Vector3.Distance(target.eulerAngles, rot.eulerAngles) < 0.01f)
                {
                    rot = target;
                    _rotation.GetEntity(i).DelAndFire<TargetRotationComponent>();
                }
            }
        }
    }
}