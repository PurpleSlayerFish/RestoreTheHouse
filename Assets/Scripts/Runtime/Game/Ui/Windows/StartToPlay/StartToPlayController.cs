using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Services.AnalyticsService;
using SimpleUi.Abstracts;
using SimpleUi.Signals;
using UniRx;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.StartToPlay 
{
    public class StartToPlayController : UiController<StartToPlayView>, IInitializable
    {
        [Inject] private IAnalyticsService _analyticsService;
        private readonly SignalBus _signalBus;
        private readonly EcsWorld _world;

        public StartToPlayController(SignalBus signalBus, EcsWorld world)
        {
            _signalBus = signalBus;
            _world = world;
        }
        
        public void Initialize()
        {
            View.StartToPlay.OnClickAsObservable().Subscribe(x => OnStart()).AddTo(View.StartToPlay);
        }
        
        private void OnStart()
        {
            _signalBus.OpenWindow<GameHudWindow>();
            _world.SetStage(EGameStage.Play);
            _analyticsService.SendRequest("level_start");
        }
    }
}