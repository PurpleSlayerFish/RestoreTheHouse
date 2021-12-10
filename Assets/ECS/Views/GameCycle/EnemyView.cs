using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Impls;
using ECS.Views.Impls;
using Leopotam.Ecs;
using PdUtils;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class EnemyView : LinkableView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private StopPointView _activator;
        [SerializeField] private float _movementSpeed = 16f;

        private int Attack = -1;
        private int Death = -2;
        private static readonly int Stage = Animator.StringToHash("Stage");

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent>().Value = _movementSpeed;
            entity.Get<PositionComponent>().Value = Transform.position;
            entity.Get<RotationComponent>().Value = Transform.rotation;
            entity.Get<UidLinkComponent>().Link = _activator.GetEntity().Get<UIdComponent>().Value;
        }

        public void SetAttackAnim()
        {
            _animator.SetFloat(Stage, Attack);
        }
    }
}