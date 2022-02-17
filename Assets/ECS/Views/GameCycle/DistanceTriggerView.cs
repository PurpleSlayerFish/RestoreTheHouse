using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class DistanceTriggerView : LinkableView
    {
        [SerializeField] private GameObject[] _unlockable;
        [SerializeField] private GameObject[] _lockable;
        [SerializeField] private LinkableView[] _disabledViews;
        [SerializeField] private float _triggerDistance = 2.5f;
        [SerializeField] private bool _isOnStopMoving;

        public ref bool IsOnStopMoving()
        {
            return ref _isOnStopMoving;
        }

        public ref LinkableView[] GetViews()
        {
            return ref _disabledViews;
        }
        
        public ref GameObject[] GetLockable()
        {
            return ref _lockable;
        }
        
        public ref GameObject[] GetUnlockable()
        {
            return ref _unlockable;
        }
        
        public ref float GetTriggerDistance()
        {
            return ref _triggerDistance;
        }
    }
}