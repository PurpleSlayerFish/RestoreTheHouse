using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class DistanceTriggerView : LinkableView
    {
        [SerializeField] private bool _isOnStopMoving;
        [SerializeField] private Transform[] _unlockable;
        [SerializeField] private Transform[] _lockable;
        
        public void Handle()
        {
            foreach (var unlockable in _unlockable)
                unlockable.gameObject.SetActive(true);
            foreach (var lockable in _lockable)
                lockable.gameObject.SetActive(false);
        }

        public ref bool IsOnStopMoving()
        {
            return ref _isOnStopMoving;
        }
        
    }
}