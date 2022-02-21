using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class PlayerTakeHitSystem : IEcsUpdateSystem
    {
        [Inject] private SignalBus _signalBus;
        
#pragma warning disable 649
        private readonly EcsWorld _world;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PlayerComponent, LinkComponent, HpComponent, EventAddPlayerHit>.Exclude<InvincibleComponent, IsDeadComponent> _player;
        private readonly EcsFilter<PlayerComponent, LinkComponent, ElapsedTimeComponent>.Exclude<EventAddPlayerHit> _playerAfterEvent;
        private readonly EcsFilter<PlayerComponent, LinkComponent, EventHpUpdateComponent> _hpUpdate;
        private readonly EcsFilter<CameraComponent, LinkComponent> _cameraF;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private PlayerView _playerView;
        private Camera _camera;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _cameraF)
            {
                _camera = _cameraF.Get2(i).Get<CameraView>().GetCamera();
            }
            
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).View as PlayerView;
                _playerEntity = _player.GetEntity(i);
                ref var eevent = ref _player.Get4(i);
                ref var hp = ref _player.Get3(i);
                _playerView.GetRigidbody().AddForce(eevent.Knockback, ForceMode.VelocityChange);
                _playerView.GetRenderer().material = _playerView.GetDamagedMaterial();
                _playerEntity.Get<EventHpUpdateComponent>();
                hp.Value -= eevent.Damage;
                if (hp.Value <= 0)
                {
                    _playerEntity.Get<IsDeadComponent>();
                    _playerView.SetDeathAnimation();
                    _world.SetStage(EGameStage.Lose);
                    break;
                }
                _playerEntity.Get<ElapsedTimeComponent>();
                _playerEntity.Get<InvincibleComponent>();
                _playerEntity.Del<EventAddPlayerHit>();
                _playerView.SetTakeHitAnimation();
            }

            foreach (var i in _playerAfterEvent)
            {
                _playerView = _playerAfterEvent.Get2(i).View as PlayerView;
                _playerEntity = _playerAfterEvent.GetEntity(i);
                if (_playerAfterEvent.Get3(i).Value < _playerView.GetAfterDamageInvincibleDuration())
                    continue;
                _playerView.GetRenderer().material = _playerView.GetOriginMaterial();
                _playerEntity.Del<InvincibleComponent>();
                _playerEntity.Del<ElapsedTimeComponent>();
            }

            foreach (var i in _hpUpdate)
            {
                _playerView = _hpUpdate.Get2(i).View as PlayerView;
                _playerEntity = _hpUpdate.GetEntity(i);
                
                _signalBus.Fire(new SignalHpUpdate(_playerEntity.Get<HpComponent>().Value, _camera.WorldToScreenPoint(_playerView.Transform.position + Vector3.up * 6)));
                // _playerEntity.Del<EventHpUpdateComponent>();
            }
        }
    }

    public struct HpComponent
    {
        public int Value;
    }
    
    public struct EventHpUpdateComponent
    {
    }

    public struct InvincibleComponent : IEcsIgnoreInFilter
    {
    }
    
    public struct IsDeadComponent : IEcsIgnoreInFilter
    {
    }
}