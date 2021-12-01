using DataBase.Game;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
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
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly SignalBus _signalBus;
        private readonly EcsWorld _world;
        
        private const int _maxProgression = 8;
        private const int _meatForEachProgression = 15;
        private const int _price = 250;

        public StartToPlayController(SignalBus signalBus, EcsWorld world)
        {
            _signalBus = signalBus;
            _world = world;
            
        }
        
        public void Initialize()
        {
            View.StartToPlay.OnClickAsObservable().Subscribe(x => OnStart()).AddTo(View.StartToPlay);
            View.PiranhaProgression.OnClickAsObservable().Subscribe(x => OnPiranhaProgression()).AddTo(View.StartToPlay);
            View.MeatProgression.OnClickAsObservable().Subscribe(x => OnMeatProgression()).AddTo(View.StartToPlay);
            UpdateUi();
        }
        
        private void OnStart()
        {
            _signalBus.OpenWindow<GameHudWindow>();
            _signalBus.Fire(new SignalPlayerAnimation());
            _world.SetStage(EGameStage.Play);
            Amplitude.Instance.logEvent("level_start");
        }

        private void UpdateUi()
        {
            View.UpdateUi(_commonPlayerData.GetData(), _price, _maxProgression, _meatForEachProgression);
        }
        
        private void OnPiranhaProgression()
        {
            var playerData = _commonPlayerData.GetData();
            if (playerData.Coins < _price)
                return;
            if (playerData.PiranhasProgression >= _maxProgression)
                return;
            playerData.Coins -= _price;
            playerData.PiranhasProgression++;
            var impactEntity = _world.NewEntity();
            impactEntity.Get<ImpactComponent>().Value = 1;
            impactEntity.Get<ImpactTypeComponent>();
            impactEntity.Get<AddImpactEventComponent>();
            _commonPlayerData.Save(playerData);
            Amplitude.Instance.logEvent("piranha_progression_up");
            UpdateUi();
        }

        private void OnMeatProgression()
        {
            var playerData = _commonPlayerData.GetData();
            if (playerData.Coins < _price)
                return;
            if (playerData.MeatProgression >= _maxProgression * _meatForEachProgression)
                return;
            playerData.Coins -= _price;
            playerData.MeatProgression += _meatForEachProgression;
            _commonPlayerData.Save(playerData);
            Amplitude.Instance.logEvent("meat_progression_up");
            UpdateUi();
        }
    }
}