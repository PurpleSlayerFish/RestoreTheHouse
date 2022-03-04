using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class WorkerInitSystem : ReactiveSystem<EventAddComponent<WorkerComponent>>
    {
        [Inject] private ScreenVariables _screenVariables;

        private const string Path = "Path";
        private int _counter = 0;
        protected override EcsFilter<EventAddComponent<WorkerComponent>> ReactiveFilter { get; }
        protected override bool DeleteEvent => true;

        protected override void Execute(EcsEntity entity)
        {
            _counter++;
            var child = _screenVariables.GetTransformPoint(Path + _counter).GetChild(0);
            entity.Get<PositionComponent>().Value = child.position;
            var view = entity.Get<LinkComponent>().Get<WorkerView>();
            view.SetTarget(child.GetComponent<PathPointView>());
            entity.Get<TargetPositionComponent>().Value = view.GetTargetPointPosition();
        }
    }
}