using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.Impls
{
    [RequireComponent(typeof(Camera))]
    public class CameraView : LinkableView
    {
        private Camera _camera;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _camera = GetComponent<Camera>();
        }

        public ref Camera GetCamera()
        {
            return ref _camera;
        }
    }
}