using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Services.Uid;
using UnityEngine;

namespace ECS.Utils.Extensions
{
    public static class GameExtensions
    {
        public static void CreatePlayer(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.Get<RotationComponent>().Value = Quaternion.identity;
            entity.Get<ImpactComponent>().Value = 0;
            entity.Get<SpeedComponent<PositionComponent>>();
            entity.GetAndFire<PrefabComponent>().Value = "Player";
            entity.GetAndFire<PlayerComponent>();
        }

        public static void CreatePoints(this EcsWorld world)
        {
            var pathPointsArray = Object.FindObjectsOfType<PathPointView>();
            foreach (var pathPoints in pathPointsArray)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<PathPointComponent>();
                entity.Get<PositionComponent>().Value = pathPoints.transform.position;
                entity.Get<RotationComponent>().Value = pathPoints.transform.rotation;
                pathPoints.Link(entity);
                if (pathPoints.RotationDirection != Vector3.zero)
                {
                    entity.Get<RotationDirectionComponent>().Direction = pathPoints.RotationDirection;
                    entity.Get<SpeedComponent<RotationComponent>>().Value = pathPoints.RotationSpeed;
                }
            }
        }
        
        public static void CreateResources(this EcsWorld world)
        {
            var resources = Object.FindObjectsOfType<ResourceView>();
            foreach (var resource in resources)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<ResourceComponent>();
                entity.Get<LinkComponent>().View = resource;
                resource.Link(entity);
            }
        }
        
        
        public static void CreateMarkets(this EcsWorld world)
        {
            var views = Object.FindObjectsOfType<MarketView>();
            foreach (var view in views)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<MarketComponent>();
                entity.Get<LinkComponent>().View = view;
                view.Link(entity);
            }
        }
    }
}