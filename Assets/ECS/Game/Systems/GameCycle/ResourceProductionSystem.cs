using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;

namespace ECS.Game.Systems.GameCycle
{
    public class ResourceProductionSystem :IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsWorld _world;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<BuildingComponent, LinkComponent, ResourceProductionComponent> _buildings;
#pragma warning restore 649
        
        private EcsEntity _buildingEntity;
        private BuildingView _buildingView;
        private EResourceType _type;
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var j in _buildings)
            {
                _buildingEntity = _buildings.GetEntity(j);
                _buildingView = _buildings.Get2(j).Get<BuildingView>();
                if (!_buildingView.gameObject.activeInHierarchy)
                    continue;
                // No need check distance to building, if player can't spend resources here (always or anymore)
                if (_buildingView.GetResourcesSpend() == null &&
                    _buildingView.GetResourcesSpend().gameObject.activeSelf == false)
                    continue;
                if (_buildingEntity.Get<ElapsedTimeComponent>().Value < 1 / _buildings.Get3(j).Value)
                    continue;
                _buildingEntity.Del<ElapsedTimeComponent>();
                if (!_buildingView.CheckCapacity())
                    continue;
                switch (_buildings.Get1(j).Type)
                {
                    case EBuildingType.LumberMill:
                        _type = EResourceType.Wood;
                        break;
                    case EBuildingType.ConcreteMixer:
                        _type = EResourceType.Concrete;
                        break;
                    default:
                        _type = EResourceType.Wood;
                        break;
                }
                
                var resources = _world.CreateResources(_type);
                _buildingView.AddResources(ref resources);
                resources.Get<UidLinkComponent>().Link = _buildingEntity.Get<UIdComponent>().Value;
                resources.Get<MoveTweenEventComponent>().EventType = ETweenEventType.ResourceDelivery;
            }
        }
    }
}