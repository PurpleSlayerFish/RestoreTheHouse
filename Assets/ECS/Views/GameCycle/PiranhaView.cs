using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using ECS.Game.Components.Flags;
using Ecs.Views.Linkable.Impl;
using Leopotam.Ecs;
using Services.PauseService;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PiranhaView : LinkableView, IPause
    {
        [SerializeField] private Animator _animator;
        [SerializeField] public Transform Root;
        [HideInInspector] public PiranhaView Target;
        [HideInInspector] public int _formationRowNumber;
        [HideInInspector] public bool EatCheck;

        private float _animatorSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _animatorSpeed = _animator.speed;
            EatCheck = false;
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
            Transform.DOPause();
            Root.transform.DOPause();
        }

        public void UnPause()
        {
            _animator.speed = _animatorSpeed;
            Transform.DOPlay();
            Root.transform.DOPlay();
        }

        public void InitLeeway(PiranhaView view)
        {
            Entity.Get<LeewayComponent>();
            Target = view;
        }

        public void InitTween(ref float tweenMove, ref float tweenDurationFrom, ref float tweenDurationTo)
        {
            Root.DOLocalMoveX(-tweenMove, Random.Range(tweenDurationFrom, tweenDurationTo)).SetEase(Ease.Unset)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
            Root.DOLocalMoveZ(-tweenMove, Random.Range(tweenDurationFrom, tweenDurationTo)).SetEase(Ease.Unset)
                .SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
        }

        public void KillLeeway()
        {
            Root.DOKill();
            Transform.DOKill();
            Entity.Del<LeewayComponent>();
        }

        public TweenerCore<Vector3, Vector3, VectorOptions> TweenToShark(SharkView sharkView)
        {
            return Root.transform.DOMove(sharkView.transform.position, 0.5f)
                .SetEase(Ease.Unset)
                .SetLoops(1)
                .OnUpdate(() =>
                {
                    if (sharkView == null)
                        CleanUp();
                });
        }
        
        public TweenerCore<Vector3, Vector3, VectorOptions>  TweenToMan(Vector3 pos)
        {
            return Transform.DOMove(pos, 0.7f + Random.Range(0f, 0.4f))
                .SetEase(Ease.Unset)
                .SetLoops(1);
        }

        public void InitPeaceDead()
        {
            Root.transform.DOLocalMove(new Vector3(0, 8f, 10f), 1f)
                .SetEase(Ease.Unset)
                .SetRelative(true)
                .SetLoops(1)
                .OnComplete(CleanUp);
        }

        public TweenerCore<Vector3, Vector3, VectorOptions> TweenFromSpawn(Vector3 spawnerPos, ref float duration, ref float tweenMove)
        {
            Root.localPosition = spawnerPos;
            return Root.transform.DOLocalMove(new Vector3(tweenMove, 0, tweenMove), duration + Random.Range(0f, 0.6f))
                .SetEase(Ease.Unset)
                .SetLoops(1);
        }
    }
}