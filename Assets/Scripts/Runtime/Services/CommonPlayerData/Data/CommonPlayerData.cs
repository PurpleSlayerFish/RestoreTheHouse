// ReSharper disable FieldCanBeMadeReadOnly.Global

using ECS.Game.Components.Flags;
using UnityEngine;

namespace Runtime.Services.CommonPlayerData.Data
{
    public class CommonPlayerData
    {
        public EScene Level;
        public int Coins;
        public TileComponent[] tiles;
        public float fireRate;

        public CommonPlayerData()
        {
            Level = EScene.Level_1;
            Coins = 0;
            fireRate = 1;
            InitDefaultTiles();
        }

        private void InitDefaultTiles()
        {
            tiles = new TileComponent[42];
            var row = 3;
            var column = 1;
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].TilePos = new Vector2Int(column, row);
                if (row < 2 && row > -2 && column < 4)
                {
                    tiles[i].IsLock = false;
                }
                else
                {
                    tiles[i].IsLock = true;
                }
                column++;
                if (column == 7)
                {
                    column = 1;
                    row--;
                }
            }
        }
    }
}