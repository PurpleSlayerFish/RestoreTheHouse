using System;
using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class WorkerView : LinkableView, IWalkableView
    {
        [SerializeField] private Animator _animator;
        
        [SerializeField] private Transform _resourcesStack;
        [SerializeField] private float _interactionDuration = 0.7f;

        [SerializeField] private float _movementSpeed = 1.46f;
        [SerializeField] private float _movementSpeedToAnim = 1.46f;
        
        public EWorkerStage WorkerStage;

        private PathPointView _target;
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
            WorkerStage = EWorkerStage.Idle;
            entity.Get<SpeedComponent<PositionComponent>>().Value = _movementSpeed;
            entity.Get<PositionComponent>().Value = transform.position;
            _animator.SetFloat(WalkMultiplier, (float) Math.Round(_movementSpeed / _movementSpeedToAnim, 2));
            _animator.SetFloat(CarryingWalkMultiplier, (float) Math.Round(_movementSpeed / _movementSpeedToAnim, 2));
        }

        public void SetActiveResourceStack(bool value)
        {
            _resourcesStack.gameObject.SetActive(value);
        }
        
        public ref float GetInteractionDuration()
        {
            return ref _interactionDuration;
        }

        public void SetTarget(PathPointView target)
        {
            _target = target;
        }
        
        public void SetNextTargetPoint()
        {
            _target = _target.NextTarget;
        }

        public ref Vector3 GetTargetRotationDirection()
        {
            return ref _target.RotationDirection;
        }
        
        public Vector3 GetTargetPointPosition()
        {
            return _target.transform.position;
        }
        
        public EPathPointType GetTargetPointType()
        {
            return _target.Type;
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

    public enum EWorkerStage
    {
        Idle,
        Walk,
        Carry,
        CarryingWalk
    }
}