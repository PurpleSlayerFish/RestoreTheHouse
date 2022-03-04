namespace ECS.Game.Components.General
{
    public struct MoveTweenEventComponent
    {
        public ETweenEventType EventType;
    }
    
    public enum ETweenEventType
    {
        ResourcePickUp,
        ResourceSpend,
        ResourceDelivery
    }
}