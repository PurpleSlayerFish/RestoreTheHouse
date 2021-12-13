using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using TMPro;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class EnemyView : LinkableView
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private TMP_Text _hpIndicator;
        [SerializeField] private StopPointView _activator;
        [SerializeField] private float _movementSpeed = 16f;
        [SerializeField] private int _health = 2;

        private int Attack = -1;
        private int Death = -2;
        private int Hit = -3;
        private static readonly int Stage = Animator.StringToHash("Stage");

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<SpeedComponent>().Value = _movementSpeed;
            entity.Get<PositionComponent>().Value = Transform.position;
            entity.Get<RotationComponent>().Value = Transform.rotation;
            entity.Get<HealthPointComponent>().Value = _health;
            entity.Get<UidLinkComponent>().Link = _activator.GetEntity().Get<UIdComponent>().Value;
            UpdateHp();
        }

        public void SetAttackAnim()
        {
            _animator.SetInteger(Stage, Attack);
        }

        public void InitHit()
        {
            Entity.Get<HealthPointComponent>().Value--;
            if (Entity.Get<HealthPointComponent>().Value <= 0)
                InitDeath();
            else
                UpdateHp();
        }

        private void InitDeath()
        {
            _animator.SetInteger(Stage, Death);
            Entity.Del<TargetPositionComponent>();
        }
        
        private void UpdateHp()
        {
            _hpIndicator.text = Entity.Get<HealthPointComponent>().Value.ToString();
        }
    }
}