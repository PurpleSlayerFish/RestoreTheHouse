namespace ECS.Game.Components.GameCycle
{
    public struct ResourceComponent
    {
        public EResourceType Type;
    }

    public enum EResourceType
    {
        Wood,
        Concrete,
        Money,
        Food
    }
}