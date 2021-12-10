using ECS.Views.Impls;
using Leopotam.Ecs;

namespace ECS.Views.GameCycle
{
    public class StopPointView : LinkableView
    {
        public ref EcsEntity GetEntity()
        {
            return ref Entity;
        }
    }
}