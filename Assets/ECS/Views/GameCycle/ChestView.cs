using DG.Tweening;
using ECS.Game.Components.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Services.PauseService;
using TMPro;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class ChestView : LinkableView, IPause
    {
        [SerializeField] private Transform _lid;
        [SerializeField] private Transform _reward;
        [SerializeField] private TMP_Text _hpIndicator;
        [SerializeField] private int _rewardValue = 15;
        [SerializeField] private int _health = 5;
        [SerializeField] private float _trialTime = 3f;

        private Vector3 _lidRotation = new Vector3(-140, 0, 0);
        private float _rewardPosY = 4f;
        private Sequence _seq;
        
        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            entity.Get<HealthPointComponent>().Value = _health;
            entity.Get<ImpactComponent>().Value = _rewardValue;
            UpdateHp();
        }
        
        private void UpdateHp()
        {
            _hpIndicator.text = Entity.Get<HealthPointComponent>().Value.ToString();
        }

        public ref float GetTrialTime()
        {
            return ref _trialTime;
        }

        public ref int GetRewardValue()
        {
            return ref _rewardValue;
        }

        public void InitHit()
        {
            Entity.Get<HealthPointComponent>().Value--;
            UpdateHp();
            if (Entity.Get<HealthPointComponent>().Value <= 0)
                InitTrialComplete();
        }

        private void InitTrialComplete()
        {
            if(!_hpIndicator.enabled)
                return;
            _hpIndicator.enabled = false;
            _seq = DOTween.Sequence();
            _seq.Append(_lid.DORotate(_lidRotation, 0.8f).SetRelative(true).SetEase(Ease.Linear));
            _seq.Append(_reward.DOMoveY(_rewardPosY, 1.3f).SetRelative(true).SetEase(Ease.Unset));
        }

        public void Pause()
        {
            _seq?.Pause();
        }

        public void UnPause()
        {
            _seq?.Play();
        }
    }
}