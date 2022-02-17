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
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class RopeViewSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<BallComponent, LinkComponent> _ball;
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
#pragma warning restore 649

        private BallView _ballView;
        private PlayerView _playerView;

        public void Run()
        {
            if (_gameStage.Get1(0).Value == EGameStage.Pause) return;
            
            foreach (var i in _player)
                _playerView = _player.Get2(i).View as PlayerView;
                
            foreach (var i in _ball)
            {
                _ballView = _ball.Get2(i).View as BallView;
                _ballView.GetLineRenderer().SetPosition(0, _ballView.Transform.position);
                _ballView.GetLineRenderer().SetPosition(1, _ballView.GetPlayerView().GetShackle().position);
                
                _ballView.GetArrow().localEulerAngles = new Vector3(_ballView.GetArrow().localEulerAngles.x, 
                    - Mathf.Atan2(_ballView.Transform.position.z-_playerView.Transform.position.z, _ballView.Transform.position.x-_playerView.Transform.position.x)*180 / Mathf.PI, 
                    _ballView.GetArrow().localEulerAngles.z);
            }
        }
    }
}