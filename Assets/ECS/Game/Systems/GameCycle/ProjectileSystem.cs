using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Services.ElapsedTimeService;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class ProjectileSystem : IEcsUpdateSystem
    {
        [Inject] private IElapsedTimeService _elapsedTimeService;

#pragma warning disable 649
        private EcsWorld _world;
        private EcsFilter<ProjectileLauncherComponent, LinkComponent> _launchers;
        private EcsFilter<GunComponent, LinkComponent> _gun;
        private EcsFilter<ProjectileComponent, PositionComponent, SpeedComponent> _projectiles;
#pragma warning restore 649

        private EcsEntity _projectile;

        public void Run()
        {
            UpdateProjectilePosition();
            InitProjectiles();
        }

        private void UpdateProjectilePosition()
        {
            foreach (var i in _projectiles)
            {
                _projectiles.Get2(i).Value +=
                    _projectiles.Get1(i).Direction * _projectiles.Get3(i).Value * _elapsedTimeService.GetElapsedTime();
            }
        }

        private void InitProjectiles()
        {
            foreach (var i in _gun)
            {
                var gunView = _gun.Get2(i).View as GunView;
                if (_gun.GetEntity(i).Has<IsShootingComponent>())
                {
                    foreach (var j in _launchers)
                    {
                        ref var launcher = ref _launchers.GetEntity(j);
                        ref var elapsedTime = ref launcher.Get<ElapsedTimeComponent>();
                        launcher.Get<ConditionComponent<ElapsedTimeComponent>>().Value = true;
                        if (elapsedTime.Value > 1 / _launchers.Get1(j).FireRate)
                        {
                            elapsedTime.Value -= 1 / _launchers.Get1(j).FireRate;
                            _projectile = _world.CreateProjectile();
                            _projectile.Get<PositionComponent>().Value =
                                launcher.Get<LinkComponent>().View.Transform.position;
                            _projectile.Get<ProjectileComponent>().Direction =
                                gunView.GetDirection();
                            _projectile.Get<ElapsedTimeComponent>();
                            if (!_world.GetEntity<PlayerInWorkshopComponent>().IsNull())
                                _projectile.Get<InWorkshopComponent>();
                            (_launchers.Get2(j).View as ProjectileLauncherView).PlayLaunchEffect();
                        }
                    }

                    if (_gun.GetEntity(i).Has<IsHeavyComponent>())
                    {
                        _gun.GetEntity(i).Get<ElapsedTimeComponent>();
                        gunView.DoBarrelRotation();
                    }
                }
                else
                {
                    if (_gun.GetEntity(i).Has<IsHeavyComponent>())
                        gunView.StopBarrelRotation();
                    foreach (var j in _launchers)
                        _launchers.GetEntity(j).Get<ConditionComponent<ElapsedTimeComponent>>().Value = false;
                }
            }
        }
    }
}