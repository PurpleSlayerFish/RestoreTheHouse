using ECS.Game.Components.GameCycle;

namespace Runtime.Signals
{
    public struct SignalResourceUpdate
    {
        public EResourceType Type;
        public int Value;

        public SignalResourceUpdate(EResourceType type, int value)
        {
            Type = type;
            Value = value;
        }
    }
}