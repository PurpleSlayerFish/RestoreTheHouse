using Game.SceneLoading;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using UniRx;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.GameOver 
{
    public class GameOverController : UiController<GameOverView>, IInitializable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly ISceneLoadingManager _sceneLoadingManager;
        

        public GameOverController(ISceneLoadingManager sceneLoadingManager)
        {
            _sceneLoadingManager = sceneLoadingManager;
        }
        
        public void Initialize()
        {
            View.Restart.OnClickAsObservable().Subscribe(x => OnRestart()).AddTo(View.Restart);
        }
        
        public override void OnShow()
        {
            View.Show(_commonPlayerData.GetData().Level);
            Amplitude.Instance.logEvent("level_failed");
            
        }

        private void OnRestart()
        {
            _sceneLoadingManager.ReloadScene();
        }
    }
}