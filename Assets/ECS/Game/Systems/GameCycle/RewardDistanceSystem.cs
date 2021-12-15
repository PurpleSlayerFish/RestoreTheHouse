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
        private readonly EcsFilter<RewardComponent, PositionComponent, LinkComponent> _crystals;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PlayerComponent, PositionComponent> _player;
#pragma warning restore 649

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play && _gameStage.Get1(0).Value != EGameStage.Workshop) return;

            foreach (var i in _player)
            foreach (var j in _crystals)
            {
                if (Vector3.Distance(_crystals.Get2(j).Value, _player.Get2(i).Value) <
                    (_crystals.Get3(j).View as RewardView).GetDistanceToGet())
                {
                    _crystals.GetEntity(j).Get<AddImpactEventComponent>();
                    _crystals.GetEntity(j).Get<IsDestroyedComponent>();
                }
            }
        }
    }
}