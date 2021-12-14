using System;
using ECS.Game.Components.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Utils.Extensions
{
    public static class TileFilterExtension
    {
        public static EcsEntity FindTile(this EcsFilter<TileComponent> filter, Vector2Int position)
        {
            foreach (var i in filter)
            {
                ref TileComponent tile = ref filter.Get1(i);
                if (tile.Position.x == position.x && tile.Position.y == position.y)
                    return filter.GetEntity(i);
            }
            throw new ArgumentOutOfRangeException();
        }
        
        public static EcsEntity FindTile(this EcsFilter<TileComponent, InUseComponent> filter, Vector2Int position)
        {
            foreach (var i in filter)
            {
                ref var tile = ref filter.Get1(i);
                if (tile.Position.x == position.x && tile.Position.y == position.y)
                    return filter.GetEntity(i);
            }
            throw new ArgumentOutOfRangeException();
        }
        
        public static EcsEntity FindTile(this EcsFilter<TileComponent> filter, int order)
        {
            foreach (var i in filter)
            {
                if (!filter.GetEntity(i).Has<OrderComponent>())
                    continue;
                if (filter.GetEntity(i).Get<OrderComponent>().Value != order)
                    continue;
                return filter.GetEntity(i);
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}