using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using TMPro;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class EnemyView : LinkableView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private Collider _collider;
        [SerializeField] private TMP_Text _hpIndicator;
        [SerializeField] private StopPointView _activator;
        [SerializeField] private float _movementSpeed = 16f;
        [SerializeField] private int _health = 2;
        [SerializeField] private float _staggerDuration = 0.9f;
        [SerializeField] private int _rewardHitsCount = 2;
        [SerializeField] private float _playerToLoseDistance = 2f;

        private int Walk = -1;
        private int Death = -2;
        private int Hit = -3;
        private int _currentStage;
        private static readonly int Stage = Animator.StringToHash("Stage");
        private int _currentRewardHits = 0;
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent>().Value = _movementSpeed;
            entity.Get<PositionComponent>().Value = Transform.position;
            entity.Get<RotationComponent>().Value = Transform.rotation;
            entity.Get<HealthPointComponent>().Value = _health;
            entity.Get<UidLinkComponent>().Link = _activator.GetEntity().Get<UIdComponent>().Value;
            _currentStage = 0;
            UpdateHp();
        }

        public void SetAttackAnim()
        {
            _animator.SetInteger(Stage, Walk);
            _currentStage = Walk;
        }

        public void InitHit()
        {
            Entity.Get<HealthPointComponent>().Value--;
            UpdateHp();
            if (Entity.Get<HealthPointComponent>().Value <= 0)
                InitDeath();
            else
            {
                Entity.Get<SpeedComponent>().Value = 0;
                _animator.SetInteger(Stage, Hit);
                Transform.DOKill();
                Transform.DOMove(Vector3.zero, _staggerDuration).SetRelative(true).SetEase(Ease.Unset).OnComplete(() =>
                {
                    Entity.Get<SpeedComponent>().Value = _movementSpeed;
                    _animator.SetInteger(Stage, _currentStage);
                });
                
            }
        }

        private void UpdateRewardHits()
        {
            _currentRewardHits++;
            if (_currentRewardHits >= _rewardHitsCount)
            {
                _currentRewardHits = 0;
            }
        }

        private void SpawnReward()
        {
            
        }

        private void InitDeath()
        {
            _animator.SetInteger(Stage, Death);
            Entity.Del<TargetPositionComponent>();
            _collider.enabled = false;
            Sequence seq = DOTween.Sequence();
            seq.Append(Transform.DOMove(Vector3.zero, 1.3f).SetRelative(true).SetEase(Ease.Unset));
            seq.Append(Transform.DOMove(Vector3.down, 0.7f).SetRelative(true).SetEase(Ease.Unset));
            seq.OnComplete(() => Entity.Get<IsDestroyedComponent>());
        }
        
        private void UpdateHp()
        {
            _hpIndicator.text = Entity.Get<HealthPointComponent>().Value.ToString();
        }

        public ref float GetPlayerToLoseDistance()
        {
            return ref _playerToLoseDistance;
        }
    }
}