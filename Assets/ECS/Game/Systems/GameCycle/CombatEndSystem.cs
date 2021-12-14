using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    public class CombatEndSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<EnemyComponent, InCombatComponent, LinkComponent> _enemies;
        private readonly EcsFilter<PlayerComponent, InCombatComponent, LinkComponent, SpeedComponent<PositionComponent>> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
#pragma warning restore 649

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play && _gameStage.Get1(0).Value != EGameStage.Workshop)
                return;

            foreach (var i in _player)
            {
                if (_enemies.GetEntitiesCount() > 0)
                    return;
                _player.GetEntity(i).Del<InCombatComponent>();
                _player.Get4(i).Value = (_player.Get3(i).View as PlayerView).GetMovementSpeed();
            }
        }
    }
}