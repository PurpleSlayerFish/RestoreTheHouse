using System.Collections.Generic;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class BuildingView : LinkableView
    {
        [SerializeField] private EBuildingType _type;

        [SerializeField] private Transform _resourcesSpendPoint;
        [SerializeField] private Transform _resourcesDelPoint;
        [SerializeField] private Transform _resourcesDeliveryStartPoint;
        [SerializeField] private Transform _resourcesDeliveryEndPoint;
        [SerializeField] private float _resourcesDeliveryDuration;
        [SerializeField] private Vector3 _resourcesDeliveryRotation = new Vector3(0, 90, 0);

        [SerializeField] private int _resourcesCapacity = 10;
        [SerializeField] private int _stackWidth = 5;
        [SerializeField] private float _stackOffsetX = 1;
        [SerializeField] private float _stackOffsetZ = 1;

        private List<string> _deliveredResources;
        private int _lastStackX;
        private int _lastStackZ;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<BuildingComponent>().Type = _type;
            _deliveredResources = new List<string>();
            _lastStackX = 1;
            _lastStackZ = 0;
        }

        public ref Transform GetResourcesSpend()
        {
            return ref _resourcesSpendPoint;
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
            var pos = _resourcesDeliveryEndPoint.localPosition;
            return new Vector3(pos.x + _lastStackX * _stackOffsetX, pos.y, pos.z + _lastStackZ * _stackOffsetZ);
        }

        public ref float GetResourcesDeliveryDuration()
        {
            return ref _resourcesDeliveryDuration;
        }

        public ref Vector3 GetResourcesDeliveryRotation()
        {
            return ref _resourcesDeliveryRotation;
        }

        public bool CheckCapacity()
        {
            if (_resourcesCapacity > _deliveredResources.Count)
                return true;
            for (int i = 0; i < _deliveredResources.Count; i++)
                if (_deliveredResources[i] == null)
                    return true;
            return false;
        }

        public void AddResources(ref EcsEntity resource)
        {
            var i = 0;
            _lastStackX = 1;
            _lastStackZ = 0;

            if (_deliveredResources.Count >= 1 && _deliveredResources[0] == null)
            {
                _deliveredResources[0] = resource.Get<UIdComponent>().Value.ToString();
                return;
            }

            for (; i < _deliveredResources.Count; i++)
            {
                if (_deliveredResources[i] == null)
                {
                    _deliveredResources[i] = resource.Get<UIdComponent>().Value.ToString();
                    return;
                }

                _lastStackZ++;
                if (i + 1 >= _stackWidth * _lastStackX)
                {
                    _lastStackX++;
                    _lastStackZ -= _stackWidth;
                }
            }

            if (i + 1 >= _deliveredResources.Count)
                _deliveredResources.Add(resource.Get<UIdComponent>().Value.ToString());
        }

        public void RemoveResource(ref EcsEntity resource)
        {
            for (int i = 0; i < _deliveredResources.Count; i++)
            {
                if (resource.Get<UIdComponent>().Value.ToString().Equals(_deliveredResources[i]))
                {
                    _deliveredResources[i] = null;
                    return;
                }
            }
        }
    }
}