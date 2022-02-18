namespace Runtime.Signals
{
    public struct SignalHpUpdate
    {
        public int Hp;

        public SignalHpUpdate(int hp)
        {
            Hp = hp;
        }
    }
}