using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Ui.Windows.GameOver;
using Runtime.Game.Ui.Windows.LevelComplete;
using Runtime.Game.Utils.MonoBehUtils;
using SimpleUi.Signals;
using UnityEngine;
using Zenject;

#pragma warning disable 649

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class LevelEndSystem : ReactiveSystem<ChangeStageComponent>
    {
        [Inject] private readonly ScreenVariables _screenVariables;
        [Inject] private readonly SignalBus _signalBus;
        private const string DEATH_SCENE = "DeathScene";

        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<BallComponent, LinkComponent> _ball;
        private readonly EcsFilter<CameraComponent, LinkComponent> _cameraF;
        protected override EcsFilter<ChangeStageComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => false;
        private bool disable;

        protected override void Execute(EcsEntity entity)
        {
            if (disable)
                return;
            switch (entity.Get<ChangeStageComponent>().Value)
            {
                case EGameStage.Lose:
                    HandleLevelLose();
                    disable = true;
                    break;
                case EGameStage.Complete:
                    HandleLevelComplete();
                    disable = true;
                    break;
            }
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void HandleLevelComplete()
        {
            foreach (var i in _player)
            {
                (_player.Get2(i).View as PlayerView).SetIdleAnimation();
                _player.Get2(i).View.Transform.DOMoveY(0, 1f).SetEase(Ease.Linear).SetRelative(true).OnComplete(() => _signalBus.OpenWindow<LevelCompleteWindow>());
            }
        }
        
        private void HandleLevelLose()
        {
            PlayerView _playerView = null;
            BallView _ballView = null;
            CameraView _cameraView = null;
            foreach (var i in _player)
                _playerView = _player.Get2(i).View as PlayerView;

            foreach (var i in _ball)
                _ballView = _ball.Get2(i).View as BallView;
                
            foreach (var i in _cameraF)
                _cameraView = _cameraF.Get2(i).View as CameraView;

            _playerView.Transform.DOMoveY(0, 1.5f).SetEase(Ease.Linear).SetRelative(true).OnComplete(() =>
            {
                var dif = _playerView.Transform.position - _ballView.Transform.position;
                _playerView.GetRigidbody().isKinematic = true;
                _playerView.GetRigidbody().useGravity = false;
                _ballView.GetRigidbody().isKinematic = true;
                _ballView.GetRigidbody().useGravity = false;
                _ballView.GetArrow().GetComponent<MeshRenderer>().enabled = false;
                _ballView.Transform.localScale = _ballView.Transform.localScale * 1.3f;
                _playerView.Transform.position = _screenVariables.GetTransformPoint(DEATH_SCENE).position;
                _ballView.Transform.position = _screenVariables.GetTransformPoint(DEATH_SCENE).position - dif;
                foreach (var ropeJoint in _ballView.GetRopeJoints())
                {
                    var rb = ropeJoint.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
                _playerView.GetRenderer().material = _playerView.GetOriginMaterial();
                
                _cameraView.Transform.SetParent(null);
                _cameraView.Transform.position = _playerView.Transform.position - dif /2 ;
                _cameraView.GetCamera().orthographicSize = 10f;
                
                _signalBus.OpenWindow<GameOverWindow>();
            });
        }
    }
}