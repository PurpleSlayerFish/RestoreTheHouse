using System;
using Game.SceneLoading;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using UniRx;
using Zenject;

namespace Runtime.Game.Ui.Windows.SplashScreen.Impls
{
    public class SplashScreenViewController : UiController<SplashScreenView>, IInitializable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly ISceneLoadingManager _sceneLoadingManager;
        private IDisposable _disposable = Disposable.Empty;

        public SplashScreenViewController(ISceneLoadingManager sceneLoadingManager)
        {
            _sceneLoadingManager = sceneLoadingManager;
        }
        
        public void Initialize()
        {
            // _sceneLoadingManager.LoadScene(EScene.Level_10);
            _sceneLoadingManager.LoadScene(_commonPlayerData.GetData().Level);
        }

        private void OnComplete()
        {
            _disposable.Dispose();
            // _sceneLoadingManager.LoadScene(EScene.MainMenu.ToString());
        }
    }
}