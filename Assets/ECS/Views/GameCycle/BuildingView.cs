using System.Collections.Generic;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class BuildingView : LinkableView
    {
        [SerializeField] private EBuildingType _type;
        
        [SerializeField] private Transform _resourcesDelPoint;
        [SerializeField] private Transform _resourcesSpendPoint;
        [SerializeField] private Transform _resourcesDeliveryStartPoint;
        [SerializeField] private Transform _resourcesDeliveryEndPoint;
        [SerializeField] private float _resourcesDeliveryDuration;
        
        [SerializeField] private int _resourcesCapacity = 10;
        [SerializeField] private int _stackHeight = 10;
        [SerializeField] private float _stackOffsetX = 1;
        [SerializeField] private float _stackOffsetY = 1;
        
        private List<EcsEntity> _deliveredResources;
        
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<BuildingComponent>().Type = _type;
            _deliveredResources = new List<EcsEntity>();
        }

        public Transform GetResourcesSpend()
        {
            return _resourcesSpendPoint;
        }
        
        public Vector3 GetResourcesDelPos()
        {
            return _resourcesDelPoint.position;
        }

        public Vector3 GetResourcesDeliveryStartPoint()
        {
            return _resourcesDeliveryStartPoint.position;
        }
        
        public Vector3 GetResourcesDeliveryEndPoint()
        {
            return _resourcesDeliveryEndPoint.position;
        }
        
        public ref float GetResourcesDeliveryDuration()
        {
            return ref _resourcesDeliveryDuration;
        }
        
        public bool CheckCapacity()
        {
            return _resourcesCapacity > _deliveredResources.Count;
        }
        
    }
}