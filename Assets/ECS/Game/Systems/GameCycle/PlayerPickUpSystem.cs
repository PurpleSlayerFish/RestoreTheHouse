using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Utils.MonoBehUtils;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class PlayerPickUpSystem : IEcsUpdateSystem
    {
        [Inject] private ScreenVariables _screenVariables;
        private const string LEVEL_END = "LevelEnd";
        
#pragma warning disable 649
        private readonly EcsWorld _world;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<BallComponent, LinkComponent> _ball;
        private readonly EcsFilter<PickupableComponent, LinkComponent, AidKitComponent> _pickupables;
#pragma warning restore 649

        private EcsEntity _playerEntity;
        private PlayerView _playerView;
        private EcsEntity _pickupableEntity;
        private AidKitView _pickupableView;
        private BallView _ballView;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).View as PlayerView;
                _playerEntity = _player.GetEntity(i);
            }
            
            foreach (var i in _ball)
                _ballView = _ball.Get2(i).View as BallView;
            
            foreach (var i in _pickupables)
            {
                _pickupableView = _pickupables.Get2(i).View as AidKitView;
                _pickupableEntity = _pickupables.GetEntity(i);
                if (Vector3.Distance(_playerView.Transform.position, _pickupableView.Transform.position) > _playerView.GetInteractionDistance() 
                    && Vector3.Distance(_ballView.Transform.position, _pickupableView.Transform.position) > _ballView.GetInteractionDistance())
                    continue;
                var aidKitE = _pickupableEntity;
                _pickupableView.Transform.DOMove(_playerView.Transform.position + Vector3.up, _playerView.GetInteractionDuration())
                    .SetEase(Ease.Linear).OnComplete(() =>
                    {
                        ref var hp = ref _playerEntity.Get<HpComponent>().Value;
                        hp = Mathf.Clamp(hp + aidKitE.Get<AidKitComponent>().Value, 0, _playerView.GetMaxHp());
                        _playerEntity.Get<EventHpUpdateComponent>();
                        var view = aidKitE.Get<LinkComponent>().View as AidKitView;
                        view.GetParticle().transform.position = _playerView.Transform.position + Vector3.up;
                        view.GetParticle().SetActive(true);
                        view.GetMesh().SetActive(false);
                        aidKitE.Get<IsDelayCleanUpComponent>().Delay = 3f;
                    });
                _pickupableView.GetMesh().transform.DOScale(Vector3.zero, _playerView.GetInteractionDuration());
                _pickupableView.GetGreenMesh().SetActive(false);
                _pickupableEntity.Del<PickupableComponent>();
            }
            
            if (Vector3.Distance(_playerView.Transform.position, _screenVariables.GetTransformPoint(LEVEL_END).position) > _playerView.GetInteractionDistance() 
                && Vector3.Distance(_ballView.Transform.position, _screenVariables.GetTransformPoint(LEVEL_END).position) > _ballView.GetInteractionDistance())
                return;
            _world.SetStage(EGameStage.Complete);
        }
    }

    public struct AidKitComponent
    {
        public int Value;
    }

    public struct PickupableComponent : IEcsIgnoreInFilter
    {
    }
}