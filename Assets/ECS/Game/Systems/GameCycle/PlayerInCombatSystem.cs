using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class PlayerInCombatSystem : ReactiveSystem<EventAddComponent<InCombatComponent>>
    {
#pragma warning disable 649
        private readonly EcsFilter<EnemyComponent, UidLinkComponent, LinkComponent> _enemies;
#pragma warning restore 649
        protected override bool DeleteEvent => true;
        protected override EcsFilter<EventAddComponent<InCombatComponent>> ReactiveFilter { get; }
        protected override void Execute(EcsEntity entity)
        {
            if (!entity.Has<PlayerComponent>())
                return;
            
            entity.Get<SpeedComponent>().Value = 0;
            foreach (var i in _enemies)
            {
                if (_enemies.Get2(i).Link.Equals(entity.Get<InCombatComponent>().PathPoint))
                {
                    ref var enemy = ref _enemies.GetEntity(i);
                    enemy.Get<TargetPositionComponent>().Value = entity.Get<PositionComponent>().Value;
                    (_enemies.Get3(i).View as EnemyView).SetAttackAnim();
                }
            }
        }
    }
}