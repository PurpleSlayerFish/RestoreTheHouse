using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class BallView : LinkableView
    {
        [SerializeField] private Rigidbody _rigidbody;
        [SerializeField] private SpringJoint _springJoint;
        [SerializeField] private SpringJoint _ropeEnd;
        [SerializeField] private Transform[] _ropeJoints;
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private Transform _arrow;
        [SerializeField] private float _rigidbodyPushForceMultiplier;
        [SerializeField] private float _interactionDistance = 2.5f;

        public ref float GetRigidbodyPushForceMultiplier()
        {
            return ref _rigidbodyPushForceMultiplier;
        }
        
        private PlayerView _player;

        public ref Rigidbody GetRigidbody()
        {
            return ref _rigidbody;
        }
        
        public ref SpringJoint GetSpringJoint()
        {
            return ref _springJoint;
        }
        
        public ref SpringJoint GetRopeEnd()
        {
            return ref _ropeEnd;
        }
        
        public ref Transform[] GetRopeJoints()
        {
            return ref _ropeJoints;
        }

        public ref LineRenderer GetLineRenderer()
        {
            return ref _lineRenderer;
        }
        
        public ref Transform GetArrow()
        {
            return ref _arrow;
        }
        
        public ref PlayerView GetPlayerView()
        {
            return ref _player;
        }
        
        public void SetPlayerView(PlayerView value)
        {
            _player = value;
        }
        
        public ref float GetInteractionDistance()
        {
            return ref _interactionDistance;
        }
    }
}