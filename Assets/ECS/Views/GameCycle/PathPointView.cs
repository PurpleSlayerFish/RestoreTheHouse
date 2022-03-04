using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class PathPointView : MonoBehaviour
    {
        public Vector3 RotationDirection;
        public PathPointView NextTarget;
        public EPathPointType Type;
    }

    public enum EPathPointType
    {
        Default,
        Get,
        Put
    }
}