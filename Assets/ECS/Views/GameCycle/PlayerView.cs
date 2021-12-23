using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PlayerView : LinkableView
    {
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _resourcesStack;
        [SerializeField] private Animator _animator;
        [SerializeField] private int _resourcesCapacity = 10;
        [SerializeField] private int _stackHeight = 10;
        [SerializeField] private float _stackOffsetX = 1;
        [SerializeField] private float _stackOffsetY = 1;

        [SerializeField] private float _interactionDistance = 2.5f;
        [SerializeField] private float _interactionDuration = 0.4f;

        [SerializeField] private float _sensitivity = 0.001f;
        [SerializeField] private float _movementSpeed = 5f;
        [SerializeField] private float _movementLimitX = 0.025f;
        [SerializeField] private float _movementLimitY = 0.025f;

        private readonly int Idle = 0;
        private readonly int Walk = 1;
        private readonly int Stage = Animator.StringToHash("Stage");

        private Stack<EcsEntity> _resources;
        private Stack<EcsEntity> _tempResources;
        private int _stackColumn;
        private int _stackRow;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent<PositionComponent>>().Value = _movementSpeed;
            _resources = new Stack<EcsEntity>();
            _tempResources = new Stack<EcsEntity>();
            _stackColumn = 1;
            _stackRow = 1;
        }

        public ref Transform GetRoot()
        {
            return ref _root;
        }

        public ref float GetSensitivity()
        {
            return ref _sensitivity;
        }

        public ref float GetMovementLimitX()
        {
            return ref _movementLimitX;
        }

        public ref float GetMovementLimitY()
        {
            return ref _movementLimitY;
        }

        public ref float GetInteractionDistance()
        {
            return ref _interactionDistance;
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
            AddResource(ref resource);
        }

        public void AddResource(ref EcsEntity resource)
        {
            _resources.Push(resource);
            var resTransform = resource.Get<LinkComponent>().View.Transform;
            var z = _stackColumn * _stackOffsetX;
            var y = _stackRow * _stackOffsetY;
            _stackRow++;
            if (_resources.Count >= _stackHeight * _stackColumn)
            {
                _stackColumn++;
                _stackRow = 1;
            }

            // Attach to stack
            resTransform.SetParent(_resourcesStack);
            resTransform.DOLocalMove(new Vector3(0, y, z), _interactionDuration).SetEase(Ease.Unset);
            resTransform.DOLocalRotate(Vector3.zero, _interactionDuration).SetEase(Ease.Unset);
        }

        public void RemoveResource(EResourceType type, Vector3 clearPointPos)
        {
            if (_resources.Count == 0)
                return;
            var count = 0;
            foreach (var resource in _resources)
                if (resource.Get<ResourceComponent>().Type == type)
                    count++;
            if (count == 0)
                return;

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
                    var resTransform = entity.Get<LinkComponent>().View.Transform;
                    resTransform.SetParent(null);
                    resTransform.DOLocalMove(clearPointPos, _interactionDuration).SetEase(Ease.Unset)
                        .OnComplete(() => entity.Get<IsDestroyedComponent>());
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
        }
    }
}