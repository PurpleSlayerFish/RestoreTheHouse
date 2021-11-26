using DataBase.Game;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Signals;
using SimpleUi.Abstracts;
using SimpleUi.Signals;
using UniRx;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.StartToPlay 
{
    public class StartToPlayController : UiController<StartToPlayView>, IInitializable
    {
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
            _signalBus.Fire(new SignalPlayerAnimation());
            View.gameObject.SetActive(false);
            _world.SetStage(EGameStage.Play);
            Amplitude.Instance.logEvent("level_start");
        }
    }
}