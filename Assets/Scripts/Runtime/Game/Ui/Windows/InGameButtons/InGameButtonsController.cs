using DataBase.Game;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Game.Ui.Windows.InGameMenu;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Runtime.Signals;
using Signals;
using SimpleUi.Abstracts;
using SimpleUi.Signals;
using UniRx;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.InGameButtons 
{
    public class InGameButtonsController : UiController<InGameButtonsView>, IInitializable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly SignalBus _signalBus;
        private readonly EcsWorld _world;
    
        public InGameButtonsController(SignalBus signalBus, EcsWorld world)
        {
            _signalBus = signalBus;
            _world = world;
           
        }
        
        public void Initialize()
        {
            View.InGameMenuButton.OnClickAsObservable().Subscribe(x => OnGameMenu()).AddTo(View.InGameMenuButton);
        }
        
        public override void OnShow()
        {
            View.Show(_commonPlayerData.GetData().Level, _world);
            _signalBus.GetStream<SignalUpdateImpact>().Subscribe(x => OnImpactUpdate(ref x)).AddTo(View);
        }

        private void OnGameMenu()
        {
            _signalBus.OpenWindow<InGameMenuWindow>();
            _world.SetStage(EGameStage.Pause);
        }

        private void OnImpactUpdate(ref SignalUpdateImpact signal)
        {
            
            
        }
    }
}