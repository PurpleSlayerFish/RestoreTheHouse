// ReSharper disable FieldCanBeMadeReadOnly.Global

using UnityEngine;

namespace Runtime.Services.CommonPlayerData.Data
{
    public class CommonPlayerData
    {
        public EScene Level;
        public int Coins;
        public readonly int StartPrice = 4;
        
        public int TilesProgression;
        public readonly int TilesMaxProgression = 42;
        public readonly int TilesForEachProgression = 2;
        public readonly int TilesStartCount = 9;
        
        public float FireRate;
        public int FireRateProgression;
        public readonly int FireRateMaxProgression = 20;
        public readonly float FireRateForEachProgression = 0.1f;
        public readonly float FireRateStart = 1f;
        

        public CommonPlayerData()
        {
            Level = EScene.Level_1;
            Coins = 0;
            FireRateProgression = 0;
            FireRate = FireRateStart;
            TilesProgression = TilesStartCount;
        }

        public int GetNextFireRatePrice()
        {
            return Mathf.RoundToInt(StartPrice * Mathf.Pow(2, FireRateProgression));
        }
        
        public int GetNextTilesPrice()
        {
            return Mathf.RoundToInt(StartPrice * Mathf.Pow(2, (TilesProgression - TilesStartCount)/TilesForEachProgression));
        }
    }
}