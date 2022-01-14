using DG.Tweening;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Services.PauseService;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class ResourceView : LinkableView, IPause
    {
        [SerializeField] private EResourceType _type;
        
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<ResourceComponent>().Type = _type;
        }
        
        public void Pause()
        {
            Transform.DOPause();
        }

        public void UnPause()
        {
            Transform.DOPlay();
        }
    }
}