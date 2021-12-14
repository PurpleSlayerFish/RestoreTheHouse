using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class TileInitSystem : ReactiveSystem<EventAddComponent<TileComponent>>
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [Inject] private readonly GameColors _gameColors;
        protected override bool DeleteEvent => true;
        protected override EcsFilter<EventAddComponent<TileComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            var data = _commonPlayerData.GetData();
            var view = entity.Get<LinkComponent>().View as TileView;
            ref var orderValue = ref entity.Get<OrderComponent>().Value;
            if (orderValue > data.TilesProgression)
                view.SetLocked(ref _gameColors.LockedTile);
            else
                view.SetUnlocked(ref _gameColors.UnlockedTile);

        }
    }
}