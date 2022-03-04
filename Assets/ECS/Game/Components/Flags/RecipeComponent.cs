using Leopotam.Ecs;

namespace ECS.Game.Components.Flags
{
    public struct RecipeComponent : IEcsIgnoreInFilter
    {
        public ERecipeType Type;
    }
    
    public enum ERecipeType
    {
        Default, 
        LumperMillUpgrade,
        LumberMillWorkerEmploy,
        ConcreteMixerUpgrade,
        Finish
    }
}