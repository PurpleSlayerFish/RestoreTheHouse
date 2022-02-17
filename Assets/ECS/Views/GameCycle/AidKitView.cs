using ECS.Game.Systems.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class AidKitView : LinkableView
    {
        [SerializeField] private int HealthToRestore = 50;
        [SerializeField] private GameObject _mesh;
        [SerializeField] private GameObject _greenMesh;
        [SerializeField] private GameObject _particle;
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            Entity.Get<AidKitComponent>().Value = HealthToRestore;
        }

        public ref GameObject GetParticle()
        {
            return ref _particle;
        }
        
        public ref GameObject GetMesh()
        {
            return ref _mesh;
        }
        
        public ref GameObject GetGreenMesh()
        {
            return ref _greenMesh;
        }
    }
}