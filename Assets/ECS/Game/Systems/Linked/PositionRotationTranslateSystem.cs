using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using Leopotam.Ecs;

namespace ECS.Game.Systems.Linked
{
    public class PositionRotationTranslateSystem : IEcsUpdateSystem
    {
        private readonly EcsFilter<LinkComponent, RotationComponent>.Exclude<InTweenComponent> _viewsRot;
        private readonly EcsFilter<LinkComponent, PositionComponent>.Exclude<InTweenComponent> _viewsPos;
        public void Run()
        {
            foreach (var i in _viewsPos)
            {
                ref var pos = ref _viewsPos.Get2(i).Value;
                var transform = _viewsPos.Get1(i).View.Transform;
                transform.position = pos;
            }
            foreach (var i in _viewsRot)
            {
                ref var rot = ref _viewsRot.Get2(i).Value;
                var transform = _viewsRot.Get1(i).View.Transform;
                transform.rotation = rot;
            }
        }
    }
}