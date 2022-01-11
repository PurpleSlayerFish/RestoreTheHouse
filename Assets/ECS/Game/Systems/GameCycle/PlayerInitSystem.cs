using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class PlayerInitSystem : ReactiveSystem<EventAddComponent<PlayerComponent>>
    {
        [Inject] private ScreenVariables _screenVariables;

#pragma warning disable 649
        private readonly EcsWorld _world;
#pragma warning restore 649

        private const string PlayerStart = "PlayerStart";
        protected override EcsFilter<EventAddComponent<PlayerComponent>> ReactiveFilter { get; }
        protected override bool DeleteEvent => true;

        protected override void Execute(EcsEntity entity)
        {
            _world.CreateCamera();
            InitLevelData(ref entity);
        }

        private void InitLevelData(ref EcsEntity entity)
        {
            entity.Get<PositionComponent>().Value = _screenVariables.GetTransformPoint(PlayerStart).position;
            entity.Get<RotationComponent>().Value = _screenVariables.GetTransformPoint(PlayerStart).rotation;
        }
    }
}