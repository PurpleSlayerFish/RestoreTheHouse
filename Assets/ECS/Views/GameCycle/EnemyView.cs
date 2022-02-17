using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components.General;
using ECS.Game.Systems.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class EnemyView : LinkableView, IWalkableView
    {
        [SerializeField] private bool _IsDisabled;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private Transform _root;
        [SerializeField] private GameObject _deathParticle;
        
        [SerializeField] private int _attackDamage = 10;
        [SerializeField] private float _attackKnockbackForce;
        [SerializeField] private float _attackCooldown;
        [SerializeField] private float _attackDistance = 2f;
        [SerializeField] private float _movementSpeedToAnim = 1.46f;

        private readonly int Idle = 0;
        private readonly int Walk = 1;
        private readonly int JumpToBall = 2;
        private readonly int Attack = 3;
        private readonly int TakeHit = 4;
        private readonly int Death = 5;
        private readonly int Stage = Animator.StringToHash("Stage");
        private readonly string WalkMultiplier = "WalkMultiplier";

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _rigidbody.isKinematic = false;
            _navMeshAgent.stoppingDistance = _attackDistance;
            _animator.SetFloat(WalkMultiplier, _movementSpeedToAnim);
            if (_IsDisabled)
                entity.Get<EcsDisableComponent>();
        }

        public ref int GetAttackDamage()
        {
            return ref _attackDamage;
        }
        
        public ref float GetAttackCooldown()
        {
            return ref _attackCooldown;
        }
            
        public ref float GetAttackDistance()
        {
            return ref _attackDistance;
        }
   
        public ref float GetAttackKnockbackForce()
        {
            return ref _attackKnockbackForce;
        }
        
        public ref NavMeshAgent GetNavMeshAgent()
        {
            return ref _navMeshAgent;
        }

        public void SetWalkAnimation()
        {
            if (_animator.GetInteger(Stage) == Walk)
                return;
            _animator.SetInteger(Stage, Walk);
        }
        
        public void SetAttackAnimation()
        {
            _animator.SetInteger(Stage, Attack);
        }

        public void SetIdleAnimation()
        {
            if (_animator.GetInteger(Stage) == Idle)
                return;
            _animator.SetInteger(Stage, Idle);
        }
        
        public void SetJumpToBallAnimation()
        {
            _animator.SetInteger(Stage, JumpToBall);
        }
        
        public void SetDeathAnimation()
        {
            _animator.SetInteger(Stage, Death);
        }
        
        public void SetTakeHitAnimation()
        {
            _animator.SetInteger(Stage, TakeHit);
        }
        public ref Rigidbody GetRigidbody()
        {
            return ref _rigidbody;
        }

        public void OnBallHit()
        {
            Kill();
        }

        public void Kill()
        {
            _deathParticle.SetActive(true);
            _root.gameObject.SetActive(false);
            Entity.Get<EcsDisableComponent>();
            Entity.Get<IsDelayCleanUpComponent>().Delay = 5f;
        }
    }

    public struct EnemyComponent : IEcsIgnoreInFilter
    {
        
    }
}