using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class PlayerViewSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, LinkComponent> _player;
#pragma warning restore 649
        public void Run()
        {
            foreach (var i in _player)
            {
                var view = _player.Get2(i).View as PlayerView;
                if (_player.GetEntity(i).Has<IsMovingComponent>())
                    view.SetWalkAnimation();
                else
                    view.SetIdleAnimation();
            }
        }
    }
}