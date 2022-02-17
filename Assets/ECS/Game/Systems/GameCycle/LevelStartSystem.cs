using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Ui;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.AnalyticsService;
using SimpleUi.Signals;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

// ReSharper disable All
#pragma warning disable 649

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class LevelStartSystem : ReactiveSystem<LevelStartEventComponent>
    {
        [Inject] private IAnalyticsService _analyticsService;
        [Inject] private SignalBus _signalBus;
        [Inject] private ScreenVariables _screenVariables;
        private const string PLAYER_START = "PlayerStart";
        private const string BALL_START = "BallStart";

        private const float MAGNITUDE_TOLERANCE = 5f;

        private readonly EcsWorld _world;
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<BallComponent, LinkComponent> _ball;

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private bool started = false;
        private PlayerView _playerView;
        private BallView _ballView;

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        protected override EcsFilter<LevelStartEventComponent> ReactiveFilter { get; }
        protected override bool DeleteEvent => true;

        protected override void Execute(EcsEntity entity)
        {
            if (started)
                return;

            _world.SetStage(EGameStage.Play);
            _signalBus.OpenWindow<GameHudWindow>();
            _analyticsService.SendRequest("level_start");
            _world.CreateCamera();
            InitPlayerAndBall(ref entity);
            started = true;
        }

        private void InitPlayerAndBall(ref EcsEntity entity)
        {
            foreach (var i in _player)
                _playerView = _player.Get2(i).View as PlayerView;

            _playerView.Transform.position = _screenVariables.GetTransformPoint(PLAYER_START).position;
            _playerView.Transform.rotation = _screenVariables.GetTransformPoint(PLAYER_START).rotation;
            _playerView.GetPushTrigger().OnTriggerEnterAsObservable().Subscribe(x => HandlePush())
                .AddTo(_disposable);


            foreach (var j in _ball)
                _ballView = _ball.Get2(j).View as BallView;

            _ballView.Transform.position = _screenVariables.GetTransformPoint(BALL_START).position;
            _ballView.Transform.rotation = _screenVariables.GetTransformPoint(BALL_START).rotation;

            _ballView.GetSpringJoint().connectedAnchor = Vector3.zero;
            _ballView.GetSpringJoint().connectedBody = _playerView.GetRigidbody();
            _ballView.GetLineRenderer().enabled = true;
            _ballView.SetPlayerView(_playerView);

            _ballView.GetRigidbody().OnCollisionEnterAsObservable().Subscribe(x => HandleBallCollision(ref x))
                .AddTo(_disposable);
        }

        private void HandlePush()
        {
            foreach (var i in _ball)
                _ballView.GetRigidbody().AddForce(_ball.Get1(i).Direction * _ballView.GetRigidbodyPushForceMultiplier(),
                    ForceMode.VelocityChange);

            foreach (var i in _player)
                (_player.Get2(i).View as PlayerView).GetPushTrigger().enabled = false;
        }

        private void HandleBallCollision(ref Collision collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
                if (_ballView.GetRigidbody().velocity.magnitude >= MAGNITUDE_TOLERANCE)
                    collision.gameObject.GetComponent<EnemyView>().OnBallHit();

            if (collision.gameObject.CompareTag("DestrictableObstacle"))
                if (_ballView.GetRigidbody().velocity.magnitude >= MAGNITUDE_TOLERANCE)
                {
                    var view = collision.gameObject.GetComponentInParent<DestructibleBlockView>();
                    view.GetMesh().SetActive(false);
                    view.GetParticle().SetActive(true);
                    view.Entity.Get<IsDelayCleanUpComponent>().Delay = 3f;
                }
        }
    }

    public struct LevelStartEventComponent : IEcsIgnoreInFilter
    {
    }
}