using ECS.Game.Components.GameCycle;
using PdUtils;

namespace Runtime.Signals
{
    public struct SignalRecipeUpdate
    {
        public EResourceType Type;
        public Uid RecipeUid;

        public SignalRecipeUpdate(EResourceType type, Uid recipeUid)
        {
            Type = type;
            RecipeUid = recipeUid;
        }
    }
}