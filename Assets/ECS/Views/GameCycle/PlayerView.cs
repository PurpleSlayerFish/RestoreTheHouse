using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components.GameCycle;
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
        
        [SerializeField] private Transform _root;
        [SerializeField] private Transform _gunPlace;
        [SerializeField] private float _movementSpeed = 16f;
        [SerializeField] private float _rotationLimitUp = 30f;
        [SerializeField] private float _rotationLimitDown = -20f;
        [SerializeField] private float _rotationLimitRight = 25f;
        [SerializeField] private float _rotationLimitLeft = -25f;
        
        private readonly Quaternion _afterRootMapping = Quaternion.Euler(0, -90, 0);
        private bool _isPathComplete;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _isPathComplete = false;
            entity.Get<SpeedComponent>().Value = _movementSpeed;
        }
        
        public void PickupGun(Transform gun)
        {
            gun.SetParent(_gunPlace);
            gun.localPosition = Vector3.zero;
            gun.localRotation = _afterRootMapping;
        }

        public void RestoreSpeed()
        {
            Entity.Get<SpeedComponent>().Value = _movementSpeed;
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