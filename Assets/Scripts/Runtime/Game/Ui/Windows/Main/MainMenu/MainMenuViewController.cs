using Game.SceneLoading;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using SimpleUi.Interfaces;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.Main.MainMenu
{
    public class MainMenuViewController : UiController<MainMenuView>, IInitializable, IDefaultSelectable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly SignalBus _signalBus;
        private readonly ISceneLoadingManager _sceneLoadingManager;

        public Selectable DefaultSelectable => View.PlayGame;

        public MainMenuViewController(SignalBus signalBus, ISceneLoadingManager sceneLoadingManager)
        {
            _signalBus = signalBus;
            _sceneLoadingManager = sceneLoadingManager;
        }

        public void Initialize()
        {
            View.PlayGame.OnClickAsObservable().Subscribe(x => OnPlayGame()).AddTo(View.PlayGame);
            View.Exit.OnClickAsObservable().Subscribe(x => Exit()).AddTo(View.Exit);
        }
        private void OnPlayGame()
        {
            // _sceneLoadingManager.LoadScene(_commonPlayerData.GetData().Level);
            _sceneLoadingManager.LoadScene(EScene.Level_1);
        }

        private void Exit() => Application.Quit();
    }
}