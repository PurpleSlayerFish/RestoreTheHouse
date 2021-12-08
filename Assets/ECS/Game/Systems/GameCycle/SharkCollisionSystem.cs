using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    public class SharkCollisionSystem : IEcsUpdateSystem
    {
        private readonly EcsFilter<DistanceComponent, LinkComponent> _sharks;
        // private readonly EcsFilter<PiranhaComponent, LinkComponent> _piranhas;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent, ImpactComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;

        // private SharkView _sharkView;
        private PlayerView _playerView;
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;
            _playerView = _player.Get2(0).View as PlayerView;
            foreach (var i in _sharks)
            {
                // _sharkView = _sharks.Get2(i).View as SharkView;
                //
                // if (_sharkView.gameObject.activeSelf && _player.Get3(0).Value.z > _sharkView.Transform.position.z + _sharkView.GetSharkDisableDistance())
                // {
                //     _sharkView.gameObject.SetActive(false);
                //     // _playerView.PiranhasUncheck();
                //     _playerView.RestoreSpeed();
                //     continue;
                // }

                // foreach (var j in _piranhas)
                // {
                //     if (_sharkView.isKilled())
                //         break;
                //     if (!_sharkView.CheckDistantX(_piranhas.Get2(j).View.Transform.position.x))
                //         continue;
                //     if (!_sharkView.CheckDistantZ(_piranhas.Get2(j).View.Transform.position.z))
                //         continue;
                //     _playerView.EatPiranha(ref _sharkView);  
                //     _sharkView.DecrementHp();
                //     if (_playerView.GetPiranhasCount() <= 0)
                //     {
                //         _gameStage.GetEntity(0).Get<ChangeStageComponent>().Value = EGameStage.Lose;
                //         return;
                //     }
                // }
            }
        }
    }
}