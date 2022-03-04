using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    public class BuildingInitSystem : ReactiveSystem<EventAddComponent<BuildingComponent>>
    {
        [Inject] private ScreenVariables _screenVariables;

        private readonly string LumberMillDefaultProduction = "LumberMillDefaultProduction";
        private readonly string ConcreteMixerDefaultProduction = "ConcreteMixerDefaultProduction";
        
        protected override EcsFilter<EventAddComponent<BuildingComponent>> ReactiveFilter { get; }
        protected override bool DeleteEvent => true;
        protected override void Execute(EcsEntity entity)
        {
            switch (entity.Get<BuildingComponent>().Type)
            {
                case EBuildingType.LumberMill:
                    entity.Get<ResourceProductionComponent>().Value =
                        _screenVariables.GetFloatValue(LumberMillDefaultProduction);
                    break;
                case EBuildingType.ConcreteMixer:
                    entity.Get<ResourceProductionComponent>().Value =
                        _screenVariables.GetFloatValue(ConcreteMixerDefaultProduction);
                    break;
            }
        }
    }
}