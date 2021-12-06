// ReSharper disable FieldCanBeMadeReadOnly.Global

using ECS.Game.Components.Flags;
using UnityEngine;

namespace Runtime.Services.CommonPlayerData.Data
{
    public class CommonPlayerData
    {
        public EScene Level;
        public int Coins;
        public int TileProgression;
        public float FireRate;

        public CommonPlayerData()
        {
            Level = EScene.Level_1;
            Coins = 0;
            FireRate = 1;
            // TileProgression = 9;
            TileProgression = 38;
        }
    }
}