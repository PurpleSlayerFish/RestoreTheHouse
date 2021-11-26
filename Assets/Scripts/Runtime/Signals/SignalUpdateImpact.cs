namespace Runtime.Signals
{
    public struct SignalUpdateImpact
    {
        public int Impact;
        public int Stage;

        public SignalUpdateImpact(int impact, int stage)
        {
            Impact = impact;
            Stage = stage;
        }
    }
}