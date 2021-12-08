// ReSharper disable FieldCanBeMadeReadOnly.Global

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
            FireRate = 1f;
            // TileProgression = 9;
            TileProgression = 38;
        }
    }
}