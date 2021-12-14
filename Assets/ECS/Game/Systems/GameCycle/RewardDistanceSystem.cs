using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class RewardDistanceSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<EnemyComponent, PositionComponent, LinkComponent> _enemies;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PlayerComponent, PositionComponent> _player;
#pragma warning restore 649

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play && _gameStage.Get1(0).Value != EGameStage.Workshop) return;

            foreach (var i in _player)
            foreach (var j in _enemies)
            {
                // if (Vector3.Distance(_enemies.Get2(j).Value, _player.Get2(i).Value) <
                //     (_enemies.Get3(j).View as EnemyView).GetPlayerToLoseDistance())
                //     _gameStage.GetEntity(0).Get<ChangeStageComponent>().Value = EGameStage.Lose;
            }
        }
    }
}