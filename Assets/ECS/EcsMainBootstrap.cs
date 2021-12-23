using System;
using System.Collections.Generic;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Zenject;

namespace ECS
{
    public class EcsMainBootstrap : IInitializable, ITickable, IDisposable
    {
        private readonly EcsWorld _world;
        private readonly EcsSystems _initUpdateSystems;
        public EcsMainBootstrap(EcsWorld world,
            IList<IEcsUpdateSystem> updateSystems,
            IList<IEcsInitSystem> initSystems)
        {
            _world = world;
            _initUpdateSystems = new EcsSystems(_world);

            if (initSystems.Count > 0)
                _initUpdateSystems.AddRange(initSystems);
            
            if (updateSystems.Count > 0)
                _initUpdateSystems.AddRange(updateSystems);
            
            _initUpdateSystems.DeclareOneFrameEvents();
        }

        public void Initialize()
        {
#if UNITY_EDITOR
            Leopotam.Ecs.UnityIntegration.EcsSystemsObserver.Create (_initUpdateSystems);
            Leopotam.Ecs.UnityIntegration.EcsWorldObserver.Create (_world);
#endif
            //Create filters for use out of systems
            _world.GetFilter(typeof(EcsFilter<UIdComponent>));
            _world.GetFilter(typeof(EcsFilter<PlayerComponent>));

            _initUpdateSystems?.Init();
        }

        public void Tick()
        {
            _initUpdateSystems?.Run();
        }

        public void Dispose()
        {
            _initUpdateSystems?.Destroy();
            _world?.Destroy();
        }
    }
}
