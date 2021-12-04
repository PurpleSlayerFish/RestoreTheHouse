using ECS.Game.Systems;
using ECS.Game.Systems.GameCycle;
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
        [SerializeField] private GameColors gameColors;
        public override void InstallBindings()
        {
            Container.Bind<ScreenVariables>().FromInstance(_screenVariables).AsSingle();
            Container.Bind<GameColors>().FromInstance(gameColors).AsSingle();
            Container.BindInterfacesAndSelfTo<EcsWorld>().AsSingle().NonLazy();
            BindSystems();
            Container.BindInterfacesTo<EcsMainBootstrap>().AsSingle();

        }

        private void BindSystems()
        {
            Container.BindInterfacesAndSelfTo<IsAvailableSetViewSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameInitializeSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<InstantiateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<PositionRotationTranslateSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameTimerSystem>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<TilesInitSystem>().AsSingle();
            // Container.BindInterfacesAndSelfTo<PiranhaAttachSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlayerForwardMovementSystem>().AsSingle();
            // Container.BindInterfacesAndSelfTo<PlayerHorizontalMovementSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<MoveRotateToTargetSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<AddImpactToPlayerSystem>().AsSingle();
            // Container.BindInterfacesAndSelfTo<LeewaySystem>().AsSingle();
            // Container.BindInterfacesAndSelfTo<SharkCollisionSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<WorkshopDragAndDropSystem>().AsSingle();

            Container.BindInterfacesAndSelfTo<LevelEndSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GamePauseSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveGameSystem>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStageSystem>().AsSingle();        //always must been last
            Container.BindInterfacesAndSelfTo<CleanUpSystem>().AsSingle();          //must been latest than last!
        }       
    }
}