using PdUtils;
using Runtime.DataBase.Game;

namespace ECS.Game.Components.GameCycle
{
    public struct InUseComponent
    {
        public Uid User;
        public EGunCubeType Type;
    }
}