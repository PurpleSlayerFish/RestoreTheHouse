using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Signals;
using TMPro;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    public class CostView : LinkableView
    {
        [Inject] public SignalBus _signalBus;
        [SerializeField] private EResourceType _resourcesType;
        [SerializeField] private RecipeView _recipeView;
        [SerializeField] private TMP_Text _countTxt;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            UpdateCountTxt();
            _signalBus.Subscribe<SignalResourceDeliver>(x =>
            {
                if (_resourcesType != x.Type)
                    return;
                if (_recipeView == null)
                    return;
                if (_recipeView.GetUid() != x.RecipeUid)
                    return;
                UpdateCountTxt();
            });
        }

        private void UpdateCountTxt()
        {
            var resources = _recipeView.GetResources();
            for (int i = 0; i < resources.Length; i++)
                if (resources[i] == _resourcesType)
                {
                    if (_recipeView.GetResourcesCount()[i] <= 0)
                        Transform.gameObject.SetActive(false);
                    else
                        _countTxt.text = _recipeView.GetResourcesCount()[i].ToString();
                    return;
                }
        }
    }
}