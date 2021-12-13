using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using Leopotam.Ecs;
using Runtime.Services.ElapsedTimeService;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.Linked
{
    public class ElapsedTimeSystem : IEcsUpdateSystem
    {
        [Inject] private IElapsedTimeService _elapsedTimeService;
        
#pragma warning disable 649
        private EcsFilter<ElapsedTimeComponent>.Exclude<ConditionComponent<ElapsedTimeComponent>> _elapsedTimeComponents;
        private EcsFilter<ElapsedTimeComponent, ConditionComponent<ElapsedTimeComponent>> _conditionElapsedTimeComponents;
        private readonly EcsFilter<GameStageComponent> _gameStage;
#pragma warning restore 649
        private float _time = Time.realtimeSinceStartup;
        private float _elapsedTime = Time.realtimeSinceStartup;

        private bool _onPause = true;
        
        public void Run()
        {

            if (_gameStage.Get1(0).Value != EGameStage.Play && _gameStage.Get1(0).Value != EGameStage.Workshop)
            {
                _onPause = true;
                return;
            }

            if (_onPause)
            {
                _onPause = false;
                _time = Time.realtimeSinceStartup;
            }
            else
            {
                _elapsedTime = Time.realtimeSinceStartup - _time;
                _time = Time.realtimeSinceStartup;
            }
            
            foreach (var i in _elapsedTimeComponents)
            {
                ref var elapsedTime = ref _elapsedTimeComponents.Get1(i);
                elapsedTime.Value += _elapsedTime;
            }
            
            foreach (var i in _conditionElapsedTimeComponents)
            {
                if (!_conditionElapsedTimeComponents.Get2(i).Value)
                    continue;
                ref var elapsedTime = ref _conditionElapsedTimeComponents.Get1(i);
                elapsedTime.Value += _elapsedTime;
            }
            
            _elapsedTimeService.SetElapsedTime(_elapsedTime);
        }
    }
}