using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class EnemyTargetSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<GameStageComponent> _gameStage;

        private readonly EcsFilter<EnemyComponent, LinkComponent>.Exclude<EcsDisableComponent> _enemies;

        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
#pragma warning restore 649

        private EcsEntity _enemyEntity;
        private EnemyView _enemyView;
        private EcsEntity _playerEntity;
        private PlayerView _playerView;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;


            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).View as PlayerView;
                _playerEntity = _player.GetEntity(i);
            }

            foreach (var i in _enemies)
            {
                _enemyView = _enemies.Get2(i).View as EnemyView;
                _enemyEntity = _enemies.GetEntity(i);

                if (Vector3.Distance(_playerView.Transform.position, _enemyView.Transform.position) >
                    _enemyView.GetAttackDistance())
                {
                    _enemyView.GetNavMeshAgent().SetDestination(_playerView.Transform.position);
                    _enemyEntity.Get<IsMovingComponent>();
                }
                else
                {
                    if (!_enemyEntity.Has<ElapsedTimeComponent>())
                    {
                        _enemyEntity.Del<IsMovingComponent>();
                        _enemyEntity.Get<ElapsedTimeComponent>();
                        _enemyView.SetAttackAnimation();
                        if (_playerEntity.Has<InvincibleComponent>())
                            continue;
                        _playerEntity.Get<EventAddPlayerHit>().Damage = _enemyView.GetAttackDamage();
                        _playerEntity.Get<EventAddPlayerHit>().Knockback = (_playerView.Transform.position - _enemyView.Transform.position) * _enemyView.GetAttackKnockbackForce();
                    }
                    else if (_enemyEntity.Get<ElapsedTimeComponent>().Value >= _enemyView.GetAttackCooldown())
                    {
                        _enemyEntity.Del<ElapsedTimeComponent>();
                    }
                }
            }
        }
    }

    public struct EventAddPlayerHit
    {
        public int Damage;
        public Vector3 Knockback;
    }
}