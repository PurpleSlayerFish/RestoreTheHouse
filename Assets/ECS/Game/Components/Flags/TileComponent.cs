using Leopotam.Ecs;

namespace ECS.Game.Components.Flags
{
    public struct TileComponent : IEcsIgnoreInFilter
    {
        public bool IsLock;
    }
}