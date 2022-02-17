using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class DestructibleBlockView : LinkableView
    {
        [SerializeField] private GameObject _mesh;
        [SerializeField] private GameObject _particle;
        
        public ref GameObject GetParticle()
        {
            return ref _particle;
        }
        
        public ref GameObject GetMesh()
        {
            return ref _mesh;
        }
    }

    public struct DestructibleBlockComponent : IEcsIgnoreInFilter
    {
        
    }
    
}