﻿using ECS.Game.Systems.GameCycle;
using ECS.Game.Systems.GameDay;
using ECS.Game.Systems.Linked;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using UnityEngine;
using Zenject;

namespace ECS.Installers
{
    public class EcsInstaller : MonoInstaller
    {
        [SerializeField] private ScreenVariables _screenVariables;
        public override void InstallBindings()
        {
            Container.Bind<ScreenVariables>().FromInstance(_screenVariables).AsSingle();
            Container.BindInterfacesAndSelfTo<EcsWorld>().AsSingle().NonLazy();
            BindSystems();
            Container.BindInterfacesTo<EcsMainBootstrap>().AsSingle();
        }

        private void BindSystems()
        {
            Container.BindInterfacesAndSelfTo<IsAvailableSetViewSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameInitializeSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<InstantiateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameTimerSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ElapsedTimeSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PositionRotationTranslateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<MoveRotateToTargetSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerInitSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<BuildingInitSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<WorkerInitSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<WorkerPathSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerMovementSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourceDistanceSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<BuildingDistanceSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<RecipeDistanceSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ClueDistanceSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<PlayerPickUpSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourceProductionSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<WalkableViewSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<ResourceViewSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<DelayCleanUpSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelEndSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePauseSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveGameSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStageSystem>().AsSingle();        //always must been last
            Container.BindInterfacesAndSelfTo<CleanUpSystem>().AsSingle();          //must been latest than last!
        }       
    }
}