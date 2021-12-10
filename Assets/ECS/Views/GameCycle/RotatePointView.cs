using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class RotatePointView : LinkableView
    {
        public Vector3 Direction;
        public float RotationSpeed = 54f;
    }
}
