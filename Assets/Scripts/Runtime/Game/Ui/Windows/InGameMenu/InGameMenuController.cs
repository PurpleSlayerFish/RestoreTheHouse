using ECS.Utils.Extensions;
using Game.SceneLoading;
using Game.Ui.BlackScreen;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Services.PauseService;
using Signals;
using SimpleUi.Abstracts;
using SimpleUi.Signals;
using UniRx;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.InGameMenu
{
    public class InGameMenuController : UiController<InGameMenuView>, IInitializable
    {
        private readonly ISceneLoadingManager _sceneLoadingManager;
        private readonly EcsWorld _world;
        private readonly SignalBus _signalBus;
        private readonly IPauseService _pauseService;

        public InGameMenuController(ISceneLoadingManager sceneLoadingManager, EcsWorld world, SignalBus signalBus, IPauseService pauseService)
        {
            _sceneLoadingManager = sceneLoadingManager;
            _world = world;
            _signalBus = signalBus;
            _pauseService = pauseService;
        }
        
        public void Initialize()
        {
            View.GoMenu.OnClickAsObservable().Subscribe(x => OnGoMenu()).AddTo(View.GoMenu);
            View.Continue.OnClickAsObservable().Subscribe(x => OnContinue()).AddTo(View.Continue);
            View.Restart.OnClickAsObservable().Subscribe(x => OnRestart()).AddTo(View.Restart);
        }

        public override void OnShow()
        {
            _pauseService.PauseGame(true);
            View.GoMenu.Select();
            View.GoMenu.OnSelect(null);
        }

        private void OnContinue()
        {
            _signalBus.BackWindow();
            _world.SetStage(EGameStage.Play);
        }

        private void OnGoMenu()
        {
            _signalBus.BackWindow();
            _signalBus.OpenWindow<BlackScreenWindow>(EWindowLayer.Project);
            _signalBus.Fire(new SignalBlackScreen(false, () =>
            {
                _sceneLoadingManager.LoadScene(EScene.MainMenu);
            }));
        }
        
        private void OnRestart()
        {
            _sceneLoadingManager.ReloadScene();
        }
    }
}