using DG.Tweening;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using Leopotam.Ecs;
using TMPro;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class SharkView : InteractableView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _indicator;
        [SerializeField] private float distanceX;
        [SerializeField] private float distanceZ;
        public float SharkDestroyDistance = 6f;
        private int _hp;
        private bool _killed;
        private const int Idle = 0;
        private const int Dead = -2;
        private const int Complete = -3;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _hp = impact;
            UpdateHp();
            _killed = false;
            SetAnimation(Idle);
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
            SetAnimation(Dead);
            _animator.speed = 2f;
            _indicator.enabled = false;
            Transform.DOLocalMove(Vector3.zero, 3f)
                .SetEase(Ease.Unset)
                .SetRelative(true)
                .SetLoops(1)
                .OnComplete(() => Entity.Get<IsDestroyedComponent>());
        }
        
        private void SetAnimation(int stage)
        {
            _animator.SetInteger("Stage", stage);
        }

        public bool isKilled()
        {
            return _killed;
        }
    }
}