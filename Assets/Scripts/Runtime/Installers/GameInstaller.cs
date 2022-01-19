using ECS.Utils.Impls;
using Runtime.Game.Ui;
using Runtime.Game.Ui.Windows.GameOver;
using Runtime.Game.Ui.Windows.InGameButtons;
using Runtime.Game.Ui.Windows.InGameMenu;
using Runtime.Game.Ui.Windows.LevelComplete;
using Runtime.Game.Ui.Windows.StartToPlay;
using Runtime.Initializers;
using Runtime.Services.AnalyticsService.Impls;
using Runtime.Services.ElapsedTimeService.Impls;
using Runtime.Services.PauseService.Impls;
using Runtime.UI.QuitConcentPopUp;
using Zenject;

namespace Installers
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindWindows();
            BindServices();
            Container.BindInterfacesAndSelfTo<GameInitializer>().AsSingle();
        }

        private void BindWindows()
        {
            Container.BindInterfacesAndSelfTo<ConsentWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<InGameMenuWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameHudWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameOverWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<LevelCompleteWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<StartToPlayWindow>().AsSingle();
            Container.BindInterfacesAndSelfTo<InGameButtonsWindow>().AsSingle();
        }

        private void BindServices()
        {
            Container.BindInterfacesTo<SpawnService>().AsSingle();
            Container.BindInterfacesTo<PauseService>().AsSingle();
            Container.BindInterfacesTo<ElapsedTimeService>().AsSingle();
            Container.BindInterfacesTo<AnalyticsService>().AsSingle();
        }
    }
}