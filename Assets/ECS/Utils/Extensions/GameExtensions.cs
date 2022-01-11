using System;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Services.Uid;
using UnityEngine;
using Object = UnityEngine.Object;

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
            entity.Get<SpeedComponent<PositionComponent>>();
            entity.Get<WalkableComponent>();
            entity.GetAndFire<PrefabComponent>().Value = "Player";
            entity.GetAndFire<PlayerComponent>();
        }
        
        public static void CreateCamera(this EcsWorld world)
        {
            var view = Object.FindObjectOfType<CameraView>(true);
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<CameraComponent>();
            entity.Get<LinkComponent>().View = view;
            view.Link(entity);
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

        public static EcsEntity CreateResources(this EcsWorld world, EResourceType type)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.GetAndFire<PrefabComponent>().Value = Enum.GetName(typeof(EResourceType), type);
            entity.Get<ResourceComponent>();
            return entity;
        }
        
        public static void CreateBuildings(this EcsWorld world)
        {
            var views = Object.FindObjectsOfType<BuildingView>(true);
            foreach (var view in views)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<LinkComponent>().View = view;
                view.Link(entity);
                entity.GetAndFire<BuildingComponent>();
            }
        }
        
        public static void CreateReceipts(this EcsWorld world)
        {
            var views = Object.FindObjectsOfType<RecipeView>(true);
            foreach (var view in views)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<RecipeComponent>();
                entity.Get<LinkComponent>().View = view;
                view.Link(entity);
            }
        }
        
        public static void CreateCosts(this EcsWorld world)
        {
            var views = Object.FindObjectsOfType<CostView>(true);
            foreach (var view in views)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<LinkComponent>().View = view;
                view.Link(entity);
            }
        }
        
        public static void CreateWorker(this EcsWorld world)
        {
            // var views = Object.FindObjectsOfType<WorkerView>(true);
            // foreach (var view in views)
            // {
            //     
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<PositionComponent>();
                entity.Get<RotationComponent>().Value = Quaternion.identity;
                entity.Get<SpeedComponent<PositionComponent>>();
                entity.Get<WalkableComponent>();
                entity.Get<TargetPositionComponent>();
                // entity.Get<LinkComponent>().View = view;
                // view.Link(entity);
                
                entity.GetAndFire<PrefabComponent>().Value = "Worker";
                entity.GetAndFire<WorkerComponent>();
            // }
        }
    }
}