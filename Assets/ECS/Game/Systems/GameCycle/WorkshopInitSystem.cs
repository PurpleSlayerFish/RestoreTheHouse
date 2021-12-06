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
    public class WorkshopInitSystem : ReactiveSystem<EventAddComponent<WorkshopComponent>>
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [Inject] private readonly GameColors _gameColors;

        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<LinkComponent, TileComponent> _tiles;
        private EcsWorld _world;
        protected override EcsFilter<EventAddComponent<WorkshopComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            var data = _commonPlayerData.GetData();
            foreach (var i in _tiles)
            {
                var tileView = _tiles.Get1(i).View as TileView;
                if ((entity.Get<LinkComponent>().View as WorkshopView).GetTilePurchaseOrder(ref tileView.GetTilePos()) > data.TileProgression)
                    tileView.SetLocked(ref _gameColors.LockedTile);
            }
        }
    }
}