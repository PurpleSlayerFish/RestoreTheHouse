using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class PlayerInWorkshopView : LinkableView
    {
        [SerializeField] private Camera _camera;
        
        public ref Camera GetCamera()
        {
            return ref _camera;
        }
    }
}