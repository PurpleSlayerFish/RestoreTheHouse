using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class ShootingSystem : IEcsUpdateSystem
    {
        private EcsWorld _world;
        private EcsFilter<ProjectileLauncherComponent> _launchers;
        private EcsFilter<GunComponent> _gun;
        private EcsFilter<ProjectileComponent, PositionComponent> _projectiles;
        private float _time = Time.realtimeSinceStartup;
        private float _elapsedTime = Time.realtimeSinceStartup;

        private int count = 0;
        private EcsEntity _projectile;
        public void Run()
        {
            UpdateTime();

            foreach (var i in _projectiles)
            {
                _projectiles.Get2(i).Value += new Vector3(_projectiles.Get1(i).Speed * _elapsedTime, 0, 0);
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
                        if (elapsedTime.Value > 1 /_launchers.Get1(i).FireRate && count < 1000)
                        {
                            elapsedTime.Value -= 1 / _launchers.Get1(i).FireRate;
                            _projectile = _world.CreateProjectile();
                            _projectile.Get<PositionComponent>().Value =
                                launcher.Get<LinkComponent>().View.Transform.position;
                            count++;
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