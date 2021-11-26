using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components.Flags;
using Ecs.Views.Linkable.Impl;
using Leopotam.Ecs;
using Runtime.Signals;
using Services.PauseService;
using UniRx;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PiranhaView : LinkableView, IPause
    {
        [Inject] private SignalBus _signalBus;

        [SerializeField] private Animator _animator;

        private float _animatorSpeed;
        private static readonly int Stage = Animator.StringToHash("Stage");

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _signalBus.GetStream<SignalPlayerAnimation>().Subscribe(x => _animator.SetInteger(Stage, -1)).AddTo(this);
            _animatorSpeed = _animator.speed;
        }

        public void CleanUp()
        {
            Entity.Get<IsDestroyedComponent>();
        }
        
        public void Pause()
        {
            _animatorSpeed = _animator.speed;
            _animator.speed = 0;
        }

        public void UnPause()
        {
            _animator.speed = _animatorSpeed;
        }
    }
}