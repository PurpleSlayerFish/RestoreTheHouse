using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class EnemyNextTargetSystem : ReactiveSystem<EventRemoveComponent<TargetPositionComponent>>
    {
        protected override bool DeleteEvent => true;
        protected override EcsFilter<EventRemoveComponent<TargetPositionComponent>> ReactiveFilter { get; }
        protected override void Execute(EcsEntity entity)
        {
            // if (!entity.Has<EnemyComponent>())
            //     return;
            //
            // var enemyView = entity.Get<LinkComponent>().View as EnemyView;
            //
            // if (enemyView.HasNextTargetPoint())
            //     return;
            //
            // if (enemyView.GetTargetEntity().Has<RotationDirectionComponent>())
            // {
            //     entity.Get<TargetRotationComponent>().Value = Quaternion.Euler(enemyView.GetTargetRotationDirection());
            //     entity.Get<TargetRotationComponent>().Speed = enemyView.GetTargetRotationSpeed();
            // }
            //
            // enemyView.SetNextTargetPoint();
            // entity.Get<TargetPositionComponent>().Value = enemyView.GetTargetPointPosition();
        }
    }
}