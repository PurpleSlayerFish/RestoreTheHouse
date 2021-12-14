﻿using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
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
            entity.Get<SpeedComponent>();
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
                if (p.TryGetComponent<RotatePointView>(out var rotatePoint))
                {
                    entity.Get<RotationDirectionComponent>().Direction = rotatePoint.Direction;
                    entity.Get<SpeedComponent>().Value = rotatePoint.RotationSpeed;
                }

                if (p.TryGetComponent<StopPointView>(out var stopPoint))
                {
                    stopPoint.Link(entity);
                    entity.Get<CombatPointComponent>();
                }
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
            entity.Get<ProjectileDeathZoneComponent>();
            entity.Get<GunCubeUpdateEventComponent>();
        }

        public static EcsEntity CreateProjectileLauncher(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.GetAndFire<PrefabComponent>().Value = "ProjectileLauncher";
            entity.GetAndFire<ProjectileLauncherComponent>();
            entity.Get<ElapsedTimeComponent>();
            entity.Get<ConditionComponent<ElapsedTimeComponent>>();
            return entity;
        }

        public static EcsEntity CreateProjectile(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.GetAndFire<PrefabComponent>().Value = "Projectile";
            entity.GetAndFire<ProjectileComponent>();
            entity.Get<SpeedComponent>();
            return entity;
        }

        public static void CreateEnemies(this EcsWorld world)
        {
            var enemies = Object.FindObjectsOfType<EnemyView>();
            foreach (var link in enemies)
            {
                var entity = world.NewEntity();
                entity.Get<UIdComponent>().Value = UidGenerator.Next();
                entity.Get<PositionComponent>();
                entity.Get<RotationComponent>();
                entity.Get<SpeedComponent>();
                entity.Get<HealthPointComponent>();
                entity.Get<UidLinkComponent>();
                entity.Get<EnemyComponent>();
                entity.Get<LinkComponent>().View = link;
                link.Link(entity);
            }
        }

        public static void CreateChest(this EcsWorld world)
        {
            var chest = Object.FindObjectOfType<ChestView>();
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<HealthPointComponent>();
            entity.Get<ChestComponent>();
            entity.Get<ImpactComponent>();
            entity.Get<LinkComponent>().View = chest;
            chest.Link(entity);
        }
    }
}