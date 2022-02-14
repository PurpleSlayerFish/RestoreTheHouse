using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Game.SceneLoading;
using Runtime.Services.AnalyticsService;
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
        [Inject] private IAnalyticsService _analyticsService;
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        
        private readonly ISceneLoadingManager _sceneLoadingManager;
        private EScene loopedLevel = EScene.Level_1;
        
        public LevelCompleteController(ISceneLoadingManager sceneLoadingManager)
        {
            _sceneLoadingManager = sceneLoadingManager;
        }
        
        public void Initialize()
        {
            View.NextLevel.OnClickAsObservable().Subscribe(x => OnNextLevel()).AddTo(View.NextLevel);
        }
        
        private void OnNextLevel()
        {
            _sceneLoadingManager.LoadScene(_commonPlayerData.GetData().Level);
        }

        public override void OnShow()
        {
            _analyticsService.SendRequest("level_complete");
            OnFinish();
        }

        private void OnFinish()
        {
            var data = _commonPlayerData.GetData();
            View.Show(data.Level);
            if (data.Level >= Enum.GetValues(typeof(EScene)).Cast<EScene>().Last())
            {
                data.Level = loopedLevel;
                _analyticsService.SendRequest("last_level_complete");
            }
            else
                data.Level++;
            _commonPlayerData.Save(data);
        }
    }
}