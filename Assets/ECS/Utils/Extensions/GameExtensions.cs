using DataBase.Game;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
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
        public static EcsEntity CreatePlayer(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.Get<PositionComponent>();
            entity.Get<RotationComponent>().Value = Quaternion.identity;
            entity.Get<ImpactComponent>().Value = 0;
            entity.GetAndFire<PrefabComponent>().Value = "Player";
            entity.GetAndFire<PlayerComponent>();
            
            var impactEntity = world.NewEntity();
            impactEntity.Get<ImpactComponent>().Value = entity.Get<ImpactComponent>().Value;
            impactEntity.Get<ImpactTypeComponent>().Value = EImpactType.Addition;
            impactEntity.Get<AddImpactEventComponent>();
            
            return entity;
        }
        public static EcsEntity CreatePirahna(this EcsWorld world)
        {
            var entity = world.NewEntity();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
            entity.GetAndFire<PrefabComponent>().Value = "Piranha";
            entity.GetAndFire<PiranhaComponent>();
            return entity;
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
        
        public static void CreateGates(this EcsWorld world)
        {
            var interFromScene = Object.FindObjectsOfType<GateView>();
            foreach (var link in interFromScene)
            {
                var entity = world.NewEntity();
                link.Link(entity);
                entity.Get<LinkComponent>().View = link;
                link.SetTriggerAction(() => entity.Get<AddImpactEventComponent>());
            }
        }
        
        public static void CreateSharks(this EcsWorld world)
        {
            var sharks = Object.FindObjectsOfType<SharkView>();
            foreach (var link in sharks)
            {
                var entity = world.NewEntity();
                link.Link(entity);
                entity.Get<LinkComponent>().View = link;
                link.SetTriggerAction(() => entity.Get<AddImpactEventComponent>());
                entity.Get<DistanceComponent>();
            }
        }
        
        
    }
}