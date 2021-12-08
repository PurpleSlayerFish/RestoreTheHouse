using System.Collections.Generic;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class GunCubeView : LinkableView
    {
        [SerializeField] private EGunCubeType _gunCubeType;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<GunCubeComponent>().Type = _gunCubeType;
            entity.Get<DefaultPositionComponent>().Value = transform.position;
        }

        public ref EcsEntity GetEntity()
        {
            return ref Entity;
        }
    }
}