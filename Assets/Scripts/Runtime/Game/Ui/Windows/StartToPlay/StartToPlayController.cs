using DataBase.Game;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using SimpleUi.Signals;
using UniRx;
using UnityEngine;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.StartToPlay 
{
    public class StartToPlayController : UiController<StartToPlayView>, IInitializable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
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
            View.FirstProgression.OnClickAsObservable().Subscribe(x => OnFireRateProgression()).AddTo(View.StartToPlay);
            View.SecondProgression.OnClickAsObservable().Subscribe(x => OnTilesProgression()).AddTo(View.StartToPlay);
            UpdateUi();
        }
        
        private void OnStart()
        {
            _signalBus.OpenWindow<GameHudWindow>();
            _world.CreatePlayer();
            _world.SetStage(EGameStage.Play);
            Amplitude.Instance.logEvent("level_start");
        }

        private void UpdateUi()
        {
            View.UpdateUi();
        }
        
        private void OnFireRateProgression()
        {
            var playerData = _commonPlayerData.GetData();
            var price = playerData.GetNextFireRatePrice();
            if (playerData.Coins < price)
                return;
            if (playerData.FireRateProgression >= playerData.FireRateMaxProgression)
                return;
            playerData.Coins -= price;
            playerData.FireRateProgression++;
            playerData.FireRate = playerData.FireRateStart + playerData.FireRateProgression * playerData.FireRateForEachProgression;
            _commonPlayerData.Save(playerData);
            _world.GetEntity<PlayerInWorkshopComponent>().Get<GunCubeUpdateEventComponent>();
            Amplitude.Instance.logEvent("speed_progression_up");
            UpdateUi();
        }

        private void OnTilesProgression()
        {
            var playerData = _commonPlayerData.GetData();
            var price = playerData.GetNextTilesPrice();
            if (playerData.Coins < price)
                return;
            if (playerData.TilesProgression >= playerData.TilesMaxProgression)
                return;
            playerData.Coins -= price;
            playerData.TilesProgression = Mathf.Clamp(playerData.TilesProgression + playerData.TilesForEachProgression, 0, playerData.TilesMaxProgression);
            _commonPlayerData.Save(playerData);
            for (int i = 0; i < playerData.TilesForEachProgression; i++)
            {
                var tile = ((EcsFilter<TileComponent>)_world.GetFilter(typeof(EcsFilter<TileComponent>))).FindTile(playerData.TilesProgression - i);
                tile.ReloadAndFire<TileComponent>().IsLock = false;
            }
            Amplitude.Instance.logEvent("tiles_progression_up");
            UpdateUi();
        }
    }
}