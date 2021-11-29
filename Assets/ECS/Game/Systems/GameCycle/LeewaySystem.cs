using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class LeewaySystem : IEcsUpdateSystem
    {
        private readonly EcsFilter<LeewayComponent, LinkComponent> _leeway;
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;

        private PiranhaView _piranhaView;
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;
            foreach (var i in _leeway)
            {
                _piranhaView = _leeway.Get2(i).View as PiranhaView;
                _piranhaView.transform.position = new Vector3(
                    Mathf.Lerp(_piranhaView.transform.position.x, _piranhaView.Target.transform.position.x, 0.5f)
                    , _piranhaView.transform.position.y
                    , (_player.Get2(0).View as PlayerView).CalculateFormationRowPos(ref _piranhaView._formationRowNumber));
            }
        }
    }
}