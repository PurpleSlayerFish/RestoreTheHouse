using System.Diagnostics.CodeAnalysis;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PlayerView : LinkableView
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [HideInInspector] public bool IsPathComplete;
        
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _gunPlace;
        [SerializeField] private float _speed = 6f;
        [SerializeField] private float _rotationLimitUp = 30f;
        [SerializeField] private float _rotationLimitDown = -20f;
        [SerializeField] private float _rotationLimitRight = 25f;
        [SerializeField] private float _rotationLimitLeft = -25f;
        
        private readonly Quaternion _afterRootMapping = Quaternion.Euler(0, -90, 0);
        private float _slowedSpeed;
        private float _currentSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _slowedSpeed = _speed * 0.75f;
            _currentSpeed = _speed;
        }
        
        public void PickupGun(Transform transform)
        {
            transform.SetParent(_gunPlace);
            transform.localPosition = Vector3.zero;
            transform.localRotation = _afterRootMapping;
        }

        public void RestoreSpeed()
        {
            _currentSpeed = _speed;
        }

        public ref float GetCurrentSpeed()
        {
            return ref _currentSpeed ;
        }

        public ref Transform GetRoot()
        {
            return ref _root;
        }

        public ref float GetRotationLimitDown()
        {
            return ref _rotationLimitDown;
        }
        
        public ref float GetRotationLimitUp()
        {
            return ref _rotationLimitUp;
        }
        
        public ref float GetRotationLimitRight()
        {
            return ref _rotationLimitRight;
        }
        
        public ref float GetRotationLimitLeft()
        {
            return ref _rotationLimitLeft;
        }
    }
}