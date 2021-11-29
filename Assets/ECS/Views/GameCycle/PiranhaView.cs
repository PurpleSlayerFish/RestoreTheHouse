using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Game.Components.Flags;
using Ecs.Views.Linkable.Impl;
using Leopotam.Ecs;
using Services.PauseService;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PiranhaView : LinkableView, IPause
    {

        [SerializeField] private Animator _animator;
        [SerializeField] public Transform Root;
        [HideInInspector] public PiranhaView Target;
        [HideInInspector] public int _formationRowNumber;
        
        private float _animatorSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _animatorSpeed = _animator.speed;
        }
        
        public void SetAnimation(int stage)
        {
            _animator.SetInteger("Stage", stage);
            
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

        public void InitLeeway(PiranhaView view)
        {
            Entity.Get<LeewayComponent>();
            Target = view;
        }
    }
}