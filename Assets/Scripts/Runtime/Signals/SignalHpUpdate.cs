using UnityEngine;

namespace Runtime.Signals
{
    public struct SignalHpUpdate
    {
        public int Hp;
        public Vector2 Position;

        public SignalHpUpdate(int hp, Vector2 position)
        {
            Hp = hp;
            Position = position;
        }
    }
}