namespace ECS.Game.Components.GameCycle
{
    public struct BuildingComponent
    {
        public EBuildingType Type;
    }

    public enum EBuildingType
    {
        House,
        LumberMill,
        ConcreteMixer,
        TimberSaleVan
    }
}