using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.General;
using ECS.Views;
using Leopotam.Ecs;
using Runtime.DataBase.Game;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class WalkableViewSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<WalkableComponent, LinkComponent> _walkables;
        private readonly EcsFilter<GameStageComponent> _gameStage;
#pragma warning restore 649
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;
            
            foreach (var i in _walkables)
            {
                var view = _walkables.Get2(i).View as IWalkableView;
                if (_walkables.GetEntity(i).Has<IsMovingComponent>())
                {
                    if (view.IsCarrying())
                        view.SetCarryingWalkAnimation();
                    else
                        view.SetWalkAnimation();
                }
                else
                {
                    if (view.IsCarrying())
                        view.SetCarryAnimation();
                    else
                        view.SetIdleAnimation();
                }
            }
        }
    }
}