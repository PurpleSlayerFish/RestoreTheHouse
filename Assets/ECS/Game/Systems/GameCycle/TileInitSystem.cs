using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    public class TileInitSystem : ReactiveSystem<EventAddComponent<TileComponent>>
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [Inject] private readonly GameColors _gameColors;
        protected override EcsFilter<EventAddComponent<TileComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            var data = _commonPlayerData.GetData();
            
            if ( entity.Get<OrderComponent>().Value > data.TileProgression)
                (entity.Get<LinkComponent>().View as TileView).SetLocked(ref _gameColors.LockedTile);
        }
    }
}