using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class PlayerInitSystem : ReactiveSystem<EventAddComponent<PlayerComponent>>
    {
        [Inject] private ScreenVariables _screenVariables;
        
#pragma warning disable 649
        private EcsFilter<PlayerInWorkshopComponent, LinkComponent> _playerInWorkhsop;
        private EcsFilter<ProjectileComponent> _projectiles;
        private EcsFilter<GunComponent, LinkComponent> _gun;
        private EcsFilter<GunCubeComponent, LinkComponent, InPlaceComponent> _gunCubes;
#pragma warning restore 649

        private const string PlayerStart = "PlayerStart";
        private const string Level = "Level";
        private const string Workshop = "Workshop";
        protected override EcsFilter<EventAddComponent<PlayerComponent>> ReactiveFilter { get; }
        protected override bool DeleteEvent => true;

        protected override void Execute(EcsEntity entity)
        {
            InitLevelData(ref entity);
            DestroyAllProjectiles();
            SetupCurrentCamera();
            RebuildGun(ref entity);
        }
        
        private void DestroyAllProjectiles()
        {
            foreach (var i in _projectiles)
                _projectiles.GetEntity(i).Get<IsDestroyedComponent>();
        }

        private void SetupCurrentCamera()
        {
            foreach (var i in _playerInWorkhsop)
            {
                Camera.SetupCurrent((_playerInWorkhsop.Get2(i).View as PlayerInWorkshopView).GetCamera());
                _playerInWorkhsop.GetEntity(i).Get<IsDestroyedComponent>();
            }
        }

        private void RebuildGun(ref EcsEntity entity)
        {
            foreach (var i in _gun)
            {
                foreach (var j in _gunCubes)
                {
                    _gunCubes.GetEntity(j).Del<PositionComponent>();
                    _gunCubes.Get2(j).View.Transform.SetParent(_gun.Get2(i).View.Transform);
                }
                // _gun.GetEntity(i).Del<IsShootingComponent>();
                (_gun.Get2(i).View as GunView).SetCombatProjectileDeathDistance();
                (entity.Get<LinkComponent>().View as PlayerView).SetInRoot(_gun.Get2(i).View.Transform);
            }
        }

        private void InitLevelData(ref EcsEntity entity)
        {
            entity.Get<PositionComponent>().Value = _screenVariables.GetPoint(PlayerStart).position;
            entity.Get<RotationComponent>().Value = _screenVariables.GetPoint(PlayerStart).rotation;
            _screenVariables.GetPoint(Level).gameObject.SetActive(true);
            _screenVariables.GetPoint(Workshop).gameObject.SetActive(false);
        }
    }
}