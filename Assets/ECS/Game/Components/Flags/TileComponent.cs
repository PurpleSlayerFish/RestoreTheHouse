using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Components.Flags
{
    public struct TileComponent : IEcsIgnoreInFilter
    {
        public Vector2Int TilePos;
        public bool IsLock;
    }
}