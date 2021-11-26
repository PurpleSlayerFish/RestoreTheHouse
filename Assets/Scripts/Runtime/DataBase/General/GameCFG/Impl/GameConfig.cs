using UnityEngine;

namespace Runtime.DataBase.General.GameCFG.Impl
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "Settings/GameConfig", order = 0)]
    public class GameConfig : ScriptableObject, IGameConfig
    {
    }
}