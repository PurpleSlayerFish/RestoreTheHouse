using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Components.Flags
{
    public struct TileComponent : IEcsIgnoreInFilter
    {
        public bool IsLock;
        public Vector2Int Position;
    }
}