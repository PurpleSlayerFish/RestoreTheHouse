using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using Ecs.Views.Linkable.Impl;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using UnityEngine;
using Zenject;

namespace ECS.Views.Impls
{
    [RequireComponent(typeof(Camera))]
    public class CameraView : LinkableView
    {
        private const string StartTween = "CameraOverviewStart";
        private const string EndTween = "CameraLevelStart";
        private const string FollowingOffset = "CameraFollowingOffset";
        
        [Inject] private ScreenVariables _screenVariables;
        [SerializeField] private float _smoothness = 0.01f;
        [SerializeField] private float _overViewTweenDuration = 5f;
        [SerializeField] private float _zoomTweenDuration = 2f;

        // public void InitOverviewTween()
        // {
        //     Entity.Get<InTweenComponent>();
        //     Transform.position = _screenVariables.GetPoint(StartTween).position;
        //     Transform.rotation = _screenVariables.GetPoint(StartTween).rotation;
        //     Transform
        //         .DOMove(_screenVariables.GetPoint(EndTween).position, _overViewTweenDuration)
        //         .SetEase(Ease.Unset)
        //         .OnComplete(DelTween);
        // }

        public void InitZoomTween(Vector3 position, Transform zoomOffset)
        {
            Entity.Get<InTweenComponent>();
            Transform
                .DOMove(zoomOffset.localPosition + position, _zoomTweenDuration)
                .SetEase(Ease.Unset);
            Transform
                .DORotate(zoomOffset.rotation.eulerAngles, _zoomTweenDuration)
                .SetEase(Ease.Unset);
        }
        
        public void DelTween()
        {
            Entity.Del<InTweenComponent>();
            Entity.Get<RotationComponent>().Value = _screenVariables.GetPoint(FollowingOffset).rotation;
            Entity.Get<PositionComponent>().Value = _screenVariables.GetPoint(EndTween).position;
        }
    }
}