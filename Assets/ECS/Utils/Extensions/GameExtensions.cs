using DataBase.Game;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Impls;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Services.Uid;
using UnityEngine;

namespace ECS.Utils.Extensions
{
    public static class GameExtensions
    {
        public static void CreatePlayerInWorkshop(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<ImpactComponent>().Value = 0;
            entity.GetAndFire<PrefabComponent>().Value = "PlayerInWorkshop";
            entity.GetAndFire<PlayerInWorkshopComponent>();
        }

        public static void CreatePlayer(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.Get<RotationComponent>().Value = Quaternion.identity;
            entity.Get<ImpactComponent>().Value = 0;
            entity.GetAndFire<PrefabComponent>().Value = "Player";
            entity.GetAndFire<PlayerComponent>();
        }

        public static void CreatePoints(this EcsWorld world)
        {
            var pathPointsArray = Object.FindObjectsOfType<ScenePath>();
            foreach (var pathPoints in pathPointsArray)
            foreach (Transform p in pathPoints.transform)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<PathPointComponent>();
                entity.Get<PositionComponent>().Value = p.position;
                entity.Get<RotationComponent>().Value = p.rotation;
                if (p.TryGetComponent<RotatePoint>(out var component))
                    entity.Get<RotationDirectionComponent>().Value = component.RotateDirection;
            }
        }

        public static void CreateTiles(this EcsWorld world)
        {
            var tileFromScene = Object.FindObjectsOfType<TileView>();
            foreach (var link in tileFromScene)
            {
                var entity = world.NewEntity();
                link.Link(entity);
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<LinkComponent>().View = link;
                entity.Get<OrderComponent>();
                entity.GetAndFire<TileComponent>().IsLock = false;
            }
        }

        public static void CreateGunCubes(this EcsWorld world)
        {
            var cubesFromScene = Object.FindObjectsOfType<GunCubeView>();
            foreach (var link in cubesFromScene)
            {
                var entity = world.NewEntity();
                link.Link(entity);
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<LinkComponent>().View = link;
                entity.Get<GunCubeComponent>();
                entity.Get<DefaultPositionComponent>();
                entity.Get<PositionComponent>().Value = link.transform.position;
            }
        }

        public static void CreateGun(this EcsWorld world)
        {
            var gunView = Object.FindObjectOfType<GunView>();
            var entity = world.NewEntity();
            gunView.Link(entity);
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<LinkComponent>().View = gunView;
            entity.Get<GunComponent>();
            entity.Get<IsShootingComponent>();
            entity.Get<GunCubeUpdateEventComponent>();
        }
        
        public static EcsEntity CreateProjectileLauncher(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.GetAndFire<PrefabComponent>().Value = "ProjectileLauncher";
            entity.GetAndFire<ProjectileLauncherComponent>();
            return entity;
        }
        
        public static EcsEntity CreateProjectile(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.GetAndFire<PrefabComponent>().Value = "Projectile";
            entity.Get<ProjectileComponent>();
            return entity;
        }
    }
}