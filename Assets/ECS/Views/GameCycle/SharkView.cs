using DG.Tweening;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using Leopotam.Ecs;
using Services.PauseService;
using TMPro;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class SharkView : InteractableView, IPause
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _indicator;
        [SerializeField] private float distanceX;
        [SerializeField] private float distanceZ;
        [SerializeField] private float deathDuration = 1.3f;
        [SerializeField] private float _sharkDisableDistance = 6f;
        private int _hp;
        private bool _killed;
        private const int Idle = 0;
        private float _animatorSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _hp = impact;
            UpdateHp();
            _killed = false;
            _animator.SetInteger("Stage", Idle);
            _animatorSpeed = _animator.speed;
        }

        private void UpdateHp()
        {
            _indicator.text = _hp.ToString();
            if (_hp <= 0)
                Kill();
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            Entity.Get<SharkCollisionComponent>();
        }

        public bool CheckDistantX(float x)
        {
            return distanceX >= Mathf.Abs(Transform.position.x - x);
        }

        public bool CheckDistantZ(float z)
        {
            return distanceZ >= Mathf.Abs(Transform.position.z - z);
        }

        public void DecrementHp()
        {
            _hp--;
            UpdateHp();
        }

        public void Kill()
        {
            _killed = true;
            _killed = true;
            _animator.speed = 0;
            Transform.DOLocalRotate(new Vector3(0f, 0f, 180f), deathDuration)
                .SetEase(Ease.Unset)
                .SetRelative(true)
                .SetLoops(1)
                .OnComplete(() => Entity.Get<IsDestroyedComponent>());
            Transform.DOLocalMove(new Vector3(0, -3f, 0), deathDuration)
                .SetEase(Ease.Unset)
                .SetRelative(true)
                .SetLoops(1);
        }
        
        public bool isKilled()
        {
            return _killed;
        }

        public ref float GetSharkDisableDistance()
        {
            return ref _sharkDisableDistance;
        }
        
        
        public void Pause()
        {
            _animatorSpeed = _animator.speed;
            _animator.speed = 0;
            Transform.DOPause();
        }

        public void UnPause()
        {
            _animator.speed = _animatorSpeed;
            Transform.DOPlay();
        }
    }
}