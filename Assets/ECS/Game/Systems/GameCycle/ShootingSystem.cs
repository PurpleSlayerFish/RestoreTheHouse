using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class ShootingSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private EcsWorld _world;
        private EcsFilter<ProjectileLauncherComponent> _launchers;
        private EcsFilter<GunComponent, LinkComponent> _gun;
        private EcsFilter<ProjectileComponent, PositionComponent> _projectiles;
        private float _time = Time.realtimeSinceStartup;
        private float _elapsedTime = Time.realtimeSinceStartup;
#pragma warning restore 649

        private EcsEntity _projectile;
        public void Run()
        {
            UpdateTime();

            foreach (var i in _projectiles)
            {
                // _projectiles.Get2(i).Value += new Vector3(_projectiles.Get1(i).Speed * _elapsedTime, 0, 0);
                _projectiles.Get2(i).Value +=
                    _projectiles.Get1(i).Direction * _projectiles.Get1(i).Speed * _elapsedTime;
            }
            foreach (var i in _gun)
            {
                if (_gun.GetEntity(i).Has<IsShootingComponent>())
                {
                    foreach (var j in _launchers)
                    {
                        ref var launcher = ref _launchers.GetEntity(j);
                        ref var elapsedTime = ref launcher.Get<ElapsedTimeComponent>();
                        elapsedTime.Value += _elapsedTime;
                        if (elapsedTime.Value > 1 /_launchers.Get1(j).FireRate)
                        {
                            elapsedTime.Value -= 1 / _launchers.Get1(j).FireRate;
                            _projectile = _world.CreateProjectile();
                            _projectile.Get<PositionComponent>().Value =
                                launcher.Get<LinkComponent>().View.Transform.position;
                            _projectile.Get<ProjectileComponent>().Direction =
                                (_gun.Get2(i).View as GunView).GetDirection();
                        }
                    }
                }
                    
            }
        }
        
        private void UpdateTime()
        {
            _elapsedTime = Time.realtimeSinceStartup - _time;
            _time = Time.realtimeSinceStartup;
        }
    }
}