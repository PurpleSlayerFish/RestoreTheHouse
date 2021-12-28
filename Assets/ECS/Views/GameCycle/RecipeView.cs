using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using PdUtils;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class RecipeView : LinkableView
    {
        [SerializeField] private EResourceType[] _resources;
        [SerializeField] private int[] _resourcesCount;
        [SerializeField] private Transform[] _unlockableTransforms;
        [SerializeField] private Transform[] _lockableTransforms;
        [SerializeField] private Transform _resourcesDelPoint;
        [SerializeField] private Transform _resourcesSpendPoint;
        
        [SerializeField] private bool _finishLevel;

        public Transform GetResourcesSpend()
        {
            return _resourcesSpendPoint;
        }

        public Vector3 GetResourcesDelPos()
        {
            return _resourcesDelPoint.position;
        }
        
        public ref EResourceType[] GetResources()
        {
            return ref _resources;
        }
        
        public ref int[] GetResourcesCount()
        {
            return ref _resourcesCount;
        }

        public bool IsCompleted()
        {
            bool condition = true;
            _resourcesCount.ForEach(x => condition = condition && x <= 0);
            return condition;
        }

        public void Invoke()
        {
            foreach (var unlockableTransform in _unlockableTransforms)
                unlockableTransform.gameObject.SetActive(true);
            foreach (var lockableTransform in _lockableTransforms)
                lockableTransform.gameObject.SetActive(false);
        }

        public ref bool IsFinishLevel()
        {
            return ref _finishLevel;
        }
        
        public ref Uid GetUid()
        {
            return ref Entity.Get<UIdComponent>().Value;
        }
    }
}