// ReSharper disable FieldCanBeMadeReadOnly.Global

namespace Runtime.Services.CommonPlayerData.Data
{
    public class CommonPlayerData
    {
        public EScene Level;
        public int Coins;
        public int MeatProgression;
        public int PiranhasProgression;

        public CommonPlayerData()
        {
            Level = EScene.Level_1;
            Coins = 0;
            MeatProgression = 0;
            PiranhasProgression = 1;
        }
    }
}