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
    public class ClueDistanceSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<ClueComponent, LinkComponent> _clues;
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent> _player;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private EcsEntity _clueEntity;
        private PlayerView _playerView;
        private ClueView _clueView;

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _playerEntity = _player.GetEntity(i);

                foreach (var j in _clues)
                {
                    _clueEntity = _clues.GetEntity(j);
                    _clueView = _clues.Get2(j).Get<ClueView>();
                    if (!_clueView.gameObject.activeSelf ||
                        !_clueView.gameObject.activeInHierarchy)
                        continue;
                    if (Vector3.Distance(_clueView.Transform.position, _player.Get3(i).Value) >
                        _playerView.GetInteractionDistance())
                        continue;
                    if (_clueView.IsOnStopMoving() && _playerEntity.Has<IsMovingComponent>())
                        continue;
                    HandleClueComplete();
                }
            }
        }

        private void HandleClueComplete()
        {
            _clueView.Handle();
            _clueEntity.Get<IsDelayCleanUpComponent>().Delay = 1.5f;
        }
    }
}