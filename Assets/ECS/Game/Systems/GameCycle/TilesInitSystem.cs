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
    public class TilesInitSystem : ReactiveSystem<EventAddComponent<TileComponent>>
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [Inject] private readonly GameColors _gameColors;

        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<LinkComponent, TileComponent> _view;
        private EcsWorld _world;
        protected override EcsFilter<EventAddComponent<TileComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            foreach (var savedTile in _commonPlayerData.GetData().tiles)
            {
                ref var viewTilePos = ref (entity.Get<LinkComponent>().View as TileView).GetTilePos();
                if (savedTile.TilePos.x == viewTilePos.x && savedTile.TilePos.y == viewTilePos.y)
                {
                    entity.Get<TileComponent>().IsLock = savedTile.IsLock;
                    entity.Get<TileComponent>().TilePos = savedTile.TilePos;
                    if (savedTile.IsLock)
                        (entity.Get<LinkComponent>().View as TileView).SetLockedMaterial(ref _gameColors.LockedTile);
                    break;
                }
            }
        }
    }
}