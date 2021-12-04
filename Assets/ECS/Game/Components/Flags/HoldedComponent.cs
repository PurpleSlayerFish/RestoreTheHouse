using ECS.Views;
using Leopotam.Ecs;
using PdUtils;

namespace ECS.Game.Components.Flags
{
    public struct HoldedComponent : IEcsIgnoreInFilter
    {
        public ILinkable Target;
        // public Vector3 Target;
    }
}