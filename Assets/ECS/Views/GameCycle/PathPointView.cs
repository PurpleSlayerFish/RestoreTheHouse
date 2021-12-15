using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class PathPointView : LinkableView
    {
        public bool StartCombat;
        public Vector3 RotationDirection;
        public float RotationSpeed;
        public PathPointView NextEnemyTarget;

        public ref EcsEntity GetEntity()
        {
            return ref Entity;
        }
    }
}