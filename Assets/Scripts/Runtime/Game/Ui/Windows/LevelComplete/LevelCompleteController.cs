using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Utils.Extensions;
using Game.SceneLoading;
using Leopotam.Ecs;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using SimpleUi.Abstracts;
using UniRx;
using Utils.UiExtensions;
using Zenject;

namespace Runtime.Game.Ui.Windows.LevelComplete 
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class LevelCompleteController : UiController<LevelCompleteView>, IInitializable
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        private readonly ISceneLoadingManager _sceneLoadingManager;
        private readonly EcsWorld _world;

        public LevelCompleteController(ISceneLoadingManager sceneLoadingManager, EcsWorld world)
        {
            _sceneLoadingManager = sceneLoadingManager;
            _world = world;
        }
        
        public void Initialize()
        {
            View.NextLevel.OnClickAsObservable().Subscribe(x => OnNextLevel()).AddTo(View.NextLevel);
            View.MainMenu.OnClickAsObservable().Subscribe(x => OnMainMenu()).AddTo(View.MainMenu);
        }
        
        private void OnMainMenu() => _sceneLoadingManager.LoadScene(EScene.MainMenu);

        private void OnNextLevel()
        {
            _sceneLoadingManager.LoadScene(_commonPlayerData.GetData().Level);
        }

        public override void OnShow()
        {
            Amplitude.Instance.logEvent("level_complete");
            OnFinish();
        }

        private void OnFinish()
        {
            // var lvlImpact = _world.GetEntity<PlayerComponent>().Get<ImpactComponent>().Value;
            // var impact = lvlImpact + (lvlImpact / 100 * _commonPlayerData.GetData().MeatProgression);
            // var currentCoins = _commonPlayerData.GetData().Coins;
            // var currentLevel = _commonPlayerData.GetData().Level;
            // OnWin(impact, out var newCoinCount);
            // View.Show(impact, currentCoins, currentLevel, () => OnComplete(newCoinCount));
        }

        private void OnComplete(int newCoinCount) => View.AddCoins(View.TotalCoinIcon.anchoredPosition, newCoinCount);
        
        private void OnWin(int reward, out int newCoinCount)
        {
            var data = _commonPlayerData.GetData();
            data.Coins += reward;
            newCoinCount = data.Coins;

            if (data.Level >= Enum.GetValues(typeof(EScene)).Cast<EScene>().Last())
            {
                data.Level = EScene.Level_1;
                Amplitude.Instance.logEvent("last_level_complete");
            }
            else
                data.Level++;
            
            _commonPlayerData.Save(data);
        }
    }
}