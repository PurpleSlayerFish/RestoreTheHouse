using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using Leopotam.Ecs;

namespace ECS.Game.Systems.Linked
{
    public class DelayCleanUpSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<IsDelayCleanUpComponent> _recipes;
#pragma warning restore 649
        public void Run()
        {
            foreach (var i in _recipes)
                if (_recipes.GetEntity(i).Get<ElapsedTimeComponent>().Value > _recipes.Get1(i).Delay) _recipes.GetEntity(i).Get<IsDestroyedComponent>();
        }
    }
}