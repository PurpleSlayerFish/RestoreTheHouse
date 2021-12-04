using ECS.Game.Components;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class GunCubeView : LinkableView
    {
        private Vector3 _defaultPosition;
        
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _defaultPosition = transform.position;
        }

        public void SetDefaultPosition()
        {
            Entity.Get<PositionComponent>().Value = _defaultPosition;
        }

        public void UpdatePosition(float x, float z)
        {
            Entity.Get<PositionComponent>().Value = new Vector3(x, Entity.Get<PositionComponent>().Value.y, z);
        }
    }
}