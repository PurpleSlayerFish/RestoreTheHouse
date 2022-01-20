using Game.SceneLoading;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using Zenject;

namespace Runtime.Game.Ui.Windows.SplashScreen.Impls
{
    public class SplashScreenViewController : UiController<SplashScreenView>, IInitializable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly ISceneLoadingManager _sceneLoadingManager;

        public SplashScreenViewController(ISceneLoadingManager sceneLoadingManager)
        {
            _sceneLoadingManager = sceneLoadingManager;
        }
        
        public void Initialize()
        {
            // _sceneLoadingManager.LoadScene(EScene.Level_2);
            _sceneLoadingManager.LoadScene(_commonPlayerData.GetData().Level);
        }
    }
}