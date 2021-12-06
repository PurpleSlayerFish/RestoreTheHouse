using System;

using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    public class WorkshopView : LinkableView
    {
        private Vector2Int[] _purchaseOrder;
        private TileView[,] _tiles;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            InitPurchaseOrder();
            InitTileMatrix();
        }

        // There is might be dynamic code, but this is unreasonable.
        private void InitPurchaseOrder()
        {
            _purchaseOrder = new Vector2Int[42];
            _purchaseOrder[0] = new Vector2Int(1, 0);
            _purchaseOrder[1] = new Vector2Int(1, 1);
            _purchaseOrder[2] = new Vector2Int(1, -1);
            _purchaseOrder[3] = new Vector2Int(2, 0);
            _purchaseOrder[4] = new Vector2Int(2, 1);
            _purchaseOrder[5] = new Vector2Int(2, -1);
            _purchaseOrder[6] = new Vector2Int(3, 0);
            _purchaseOrder[7] = new Vector2Int(3, 1);
            _purchaseOrder[8] = new Vector2Int(3, -1);
            _purchaseOrder[9] = new Vector2Int(4, 0);
            _purchaseOrder[10] = new Vector2Int(4, 1);
            _purchaseOrder[11] = new Vector2Int(4, -1);
            _purchaseOrder[12] = new Vector2Int(1, 2);
            _purchaseOrder[13] = new Vector2Int(2, 2);
            _purchaseOrder[14] = new Vector2Int(3, 2);
            _purchaseOrder[15] = new Vector2Int(4, 2);
            _purchaseOrder[16] = new Vector2Int(1, -2);
            _purchaseOrder[17] = new Vector2Int(2, -2);
            _purchaseOrder[18] = new Vector2Int(3, -2);
            _purchaseOrder[19] = new Vector2Int(4, -2);
            _purchaseOrder[20] = new Vector2Int(5, 0);
            _purchaseOrder[21] = new Vector2Int(5, 1);
            _purchaseOrder[22] = new Vector2Int(5, -1);
            _purchaseOrder[23] = new Vector2Int(5, 2);
            _purchaseOrder[24] = new Vector2Int(5, -2);
            _purchaseOrder[25] = new Vector2Int(1, 3);
            _purchaseOrder[26] = new Vector2Int(2, 3);
            _purchaseOrder[27] = new Vector2Int(3, 3);
            _purchaseOrder[28] = new Vector2Int(4, 3);
            _purchaseOrder[29] = new Vector2Int(5, 3);
            _purchaseOrder[30] = new Vector2Int(1, -3);
            _purchaseOrder[31] = new Vector2Int(2, -3);
            _purchaseOrder[32] = new Vector2Int(3, -3);
            _purchaseOrder[33] = new Vector2Int(4, -3);
            _purchaseOrder[34] = new Vector2Int(5, -3);
            _purchaseOrder[35] = new Vector2Int(6, 0);
            _purchaseOrder[36] = new Vector2Int(6, 1);
            _purchaseOrder[37] = new Vector2Int(6, -1);
            _purchaseOrder[38] = new Vector2Int(6, 2);
            _purchaseOrder[39] = new Vector2Int(6, -2);
            _purchaseOrder[40] = new Vector2Int(6, 3);
            _purchaseOrder[41] = new Vector2Int(6, -3);
        }

        private void InitTileMatrix()
        {
            _tiles = new TileView[6, 7];
            var tileViews = FindObjectsOfType<TileView>();
            foreach (var tileView in tileViews)
                _tiles[tileView.GetXY().x - 1, tileView.GetXY().y + 3] = tileView;
        }

        public int GetTilePurchaseOrder(ref Vector2Int tilePos)
        {
            for (int i = 0; i < _purchaseOrder.Length; i++)
            {
                if (_purchaseOrder[i].x == tilePos.x && _purchaseOrder[i].y == tilePos.y)
                    return i;
            }
            throw new IndexOutOfRangeException();
        }

        public ref TileView GetTile(int x, int y)
        {
            return ref _tiles[x - 1, y + 3];
        }
    }
}