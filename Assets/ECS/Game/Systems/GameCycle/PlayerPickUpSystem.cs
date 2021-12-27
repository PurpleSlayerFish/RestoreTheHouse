using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class PlayerPickUpSystem : ReactiveSystem<EventAddComponent<PickedComponent>>
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
        private readonly EcsFilter<BuildingComponent, LinkComponent, UIdComponent> _building;
#pragma warning restore 649

        protected override bool DeleteEvent => true;
        protected override EcsFilter<EventAddComponent<PickedComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            foreach (var i in _player)
            {
                var playerView = _player.Get2(i).View as PlayerView;
                if (entity.Has<ResourceComponent>())
                {
                    playerView.AddResource(ref entity);
                    if (entity.Has<UidLinkComponent>())
                        foreach (var j in _building)
                            if (_building.Get3(j).Value == entity.Get<UidLinkComponent>().Link)
                            {
                                _building.Get2(j).Get<BuildingView>().RemoveResource(ref entity);
                                entity.Del<UidLinkComponent>();
                            }
                }
            }
        }
    }
}