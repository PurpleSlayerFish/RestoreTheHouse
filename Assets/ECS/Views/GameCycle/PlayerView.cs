using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Signals;
using Services.PauseService;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PlayerView : LinkableView, IWalkableView, IPause
    {
        [Inject] private SignalBus _signalBus;
        
        [SerializeField] private Animator _animator;
        
        [SerializeField] private float _interactionDistance = 2.5f;
        [SerializeField] private float _interactionDuration = 0.4f;
        [SerializeField] private float _interactionCooldown = 0.6f;

        [SerializeField] private Transform _root;
        [SerializeField] private float _sensitivity = 0.4f;
        [SerializeField] private float _movementLimit = 0.045f;
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

        private float _animationSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent<PositionComponent>>().Value = _movementSpeed;
            _animationSpeed = _animator.speed;
            _animator.SetFloat(WalkMultiplier, (float) Math.Round(_movementSpeed / _movementSpeedToAnim, 2));
            _animator.SetFloat(CarryingWalkMultiplier, (float) Math.Round(_movementSpeed / _movementSpeedToAnim, 2));
        }

        public ref Transform GetRoot()
        {
            return ref _root;
        }
        
        public ref float GetSensitivity()
        {
            return ref _sensitivity;
        }

        public ref float GetMovementLimit()
        {
            return ref _movementLimit;
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
            return false;
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

        public void Pause()
        {
            _animationSpeed = _animator.speed;
            _animator.speed = 0;
        }

        public void UnPause()
        {
            _animator.speed = _animationSpeed;
        }
    }
}