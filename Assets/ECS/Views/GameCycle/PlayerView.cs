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
        
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _resourcesStack;
        
        [SerializeField] private int _resourcesCapacity = 10;
        [SerializeField] private int _stackHeight = 10;
        [SerializeField] private float _stackOffsetX = 1;
        [SerializeField] private float _stackOffsetY = 1;

        [SerializeField] private float _interactionDistance = 2.5f;
        [SerializeField] private float _interactionDuration = 0.4f;
        [SerializeField] private float _interactionCooldown = 0.6f;

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

        private Stack<EcsEntity> _resources;
        private Stack<EcsEntity> _tempResources;
        private int _stackColumn;
        private int _stackRow;
        private float _animationSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent<PositionComponent>>().Value = _movementSpeed;
            _resources = new Stack<EcsEntity>();
            _tempResources = new Stack<EcsEntity>();
            _stackColumn = 1;
            _stackRow = 1;
            _animationSpeed = _animator.speed;
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
            return GetResourcesCount() > 0;
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

        public ref int GetResourcesCapacity()
        {
            return ref _resourcesCapacity;
        }

        public int GetResourcesCount()
        {
            return _resources.Count;
        }

        public void AddResource(EcsEntity resource)
        {
            AddResource(ref resource, false);
        }

        public void AddResource(ref EcsEntity resource, bool needUpdateUi)
        {
            _resources.Push(resource);
            var z = _stackColumn * _stackOffsetX;
            var y = _stackRow * _stackOffsetY;
            _stackRow++;
            if (_resources.Count >= _stackHeight * _stackColumn)
            {
                _stackColumn++;
                _stackRow = 1;
            }
            // Attach to stack
            resource.Get<MoveTweenEventComponent>().EventType = ETweenEventType.ResourcePickUp;
            resource.Get<Vector3Component<MoveTweenEventComponent>>().Value = new Vector3(0, y, z);
            if (needUpdateUi)
                _signalBus.Fire(new SignalResourceUpdate(resource.Get<ResourceComponent>().Type, 1));
        }

        public bool RemoveResource(EResourceType type, Vector3 clearPointPos)
        {
            if (_resources.Count == 0)
                return false;
            var count = 0;
            foreach (var resource in _resources)
                if (resource.Get<ResourceComponent>().Type == type)
                {
                    count++;
                    break;
                }
            if (count == 0)
                return false;

            bool condition = true;
            EcsEntity entity;
            while (condition)
            {
                if (_resources.Count == 0)
                    break;
                entity = _resources.Peek();
                if (entity.Get<ResourceComponent>().Type == type)
                {
                    // Detach from stack
                    entity.Get<MoveTweenEventComponent>().EventType = ETweenEventType.ResourceSpend;
                    entity.Get<Vector3Component<MoveTweenEventComponent>>().Value = clearPointPos;
                    condition = false;
                }
                else
                {
                    _tempResources.Push(entity);
                }
                _resources.Pop();
                _stackRow--;
                if (_stackRow <= 0)
                {
                    _stackColumn--;
                    _stackRow = _stackHeight;
                }
            }

            var tempCount = _tempResources.Count;
            for (int i = 0; i < tempCount; i++)
            {
                AddResource(_tempResources.Peek());
                _tempResources.Pop();
            }
            _signalBus.Fire(new SignalResourceUpdate(type, -1));
            return true;
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