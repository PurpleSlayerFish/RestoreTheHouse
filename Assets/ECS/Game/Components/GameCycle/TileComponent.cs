using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Components.GameCycle
{
    public struct TileComponent : IEcsIgnoreInFilter
    {
        public bool IsLock;
        public Vector2Int Position;
    }
}