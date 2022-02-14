using System;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Game.Systems.GameCycle;
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
        public static void CreateEcsEntities(this EcsWorld world)
        {
            // world.CreateTimer();
            world.CreatePlayer();
            // world.CreateCamera();
            world.CreateDistanceTriggers();
        }

        private static void CreateTimer(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<TimerComponent>();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
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
        
        public static void CreatePlayer(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.Get<RotationComponent>().Value = Quaternion.identity;
            entity.Get<SpeedComponent<PositionComponent>>();
            entity.Get<WalkableComponent>();
            entity.GetAndFire<PrefabComponent>().Value = "Player";
            entity.Get<PlayerComponent>();
        }

        public static void CreateDistanceTriggers(this EcsWorld world)
        {
            var views = Object.FindObjectsOfType<DistanceTriggerView>(true);
            foreach (var view in views)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<DistanceTriggerComponent>();
                entity.Get<LinkComponent>().View = view;
                view.Link(entity);
            }
        }
    }
}