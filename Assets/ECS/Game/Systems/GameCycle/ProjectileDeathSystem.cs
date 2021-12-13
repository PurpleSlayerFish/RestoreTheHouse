using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class ProjectileDeathSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<ProjectileComponent, PositionComponent, LinkComponent> _projectiles;
        private readonly EcsFilter<GunComponent, LinkComponent, ProjectileDeathZoneComponent> _gun;
        private readonly EcsFilter<GameStageComponent> _gameStage;
#pragma warning restore 649

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play && _gameStage.Get1(0).Value != EGameStage.Workshop) return;
            foreach (var j in _gun)
            {
                foreach (var i in _projectiles)
                {
                    if (Vector3.Distance(_gun.Get2(j).View.Transform.position, _projectiles.Get2(i).Value) >
                        _gun.Get3(j).Distance)
                        (_projectiles.Get3(i).View as ProjectileView).Impact();
                }
            }
        }
    }
}