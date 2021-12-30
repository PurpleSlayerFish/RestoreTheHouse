using System;
using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class WorkerView : LinkableView, IWalkableView
    {
        [SerializeField] private Animator _animator;
        
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _resourcesStack;

        [SerializeField] private float _interactionDistance = 2.5f;
        [SerializeField] private float _interactionDuration = 0.4f;
        [SerializeField] private float _interactionCooldown = 0.6f;

        [SerializeField] private float _movementSpeed = 1.46f;
        [SerializeField] private float _movementSpeedToAnim = 1.46f;
        [SerializeField] private NavMeshAgent _navMeshAgent;

        private readonly int Idle = 0;
        private readonly int Walk = 1;
        private readonly int Carry = 2;
        private readonly int CarryingWalk = 3;
        private readonly int Stage = Animator.StringToHash("Stage");
        private readonly string WalkMultiplier = "WalkMultiplier";
        private readonly string CarryingWalkMultiplier = "CarryingWalkMultiplier";

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent<PositionComponent>>().Value = _movementSpeed;
            _animator.SetFloat(WalkMultiplier, (float) Math.Round(_movementSpeed / _movementSpeedToAnim, 2));
            _animator.SetFloat(CarryingWalkMultiplier, (float) Math.Round(_movementSpeed / _movementSpeedToAnim, 2));
        }

        public ref Transform GetRoot()
        {
            return ref _root;
        }
        
        public ref Transform GetResourcesStack()
        {
            return ref _resourcesStack;
        }

        public ref NavMeshAgent GetNavMeshAgent()
        {
            return ref _navMeshAgent;
        }

        public ref float GetInteractionDistance()
        {
            return ref _interactionDistance;
        }
        
        public ref float GetInteractionDuration()
        {
            return ref _interactionDuration;
        }
        
        public ref float GetInteractionCooldown()
        {
            return ref _interactionCooldown;
        }

        public bool IsCarrying()
        {
            return _resourcesStack.gameObject.activeSelf;
        }
        
        public void SetWalkAnimation()
        {
            if (_animator.GetInteger(Stage) == Walk)
                return;
            _animator.SetInteger(Stage, Walk);
        }

        public void SetIdleAnimation()
        {
            if (_animator.GetInteger(Stage) == Idle)
                return;
            _animator.SetInteger(Stage, Idle);
            _animator.speed = 1;
        }
        
        public void SetCarryAnimation()
        {
            if (_animator.GetInteger(Stage) == Carry)
                return;
            _animator.SetInteger(Stage, Carry);
        }
        
        public void SetCarryingWalkAnimation()
        {
            if (_animator.GetInteger(Stage) == CarryingWalk)
                return;
            _animator.SetInteger(Stage, CarryingWalk);
        }
    }
}