using ECS.Game.Components.GameCycle;
using PdUtils;

namespace Runtime.Signals
{
    public struct SignalResourceDeliver
    {
        public EResourceType Type;
        public Uid RecipeUid;

        public SignalResourceDeliver(EResourceType type, Uid recipeUid)
        {
            Type = type;
            RecipeUid = recipeUid;
        }
    }
}