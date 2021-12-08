using System;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Signals;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    public class GunView : LinkableView
    {
        [Inject] private readonly SignalBus _signalBus;
        [SerializeField] private TMP_Text _workshopDpsValue;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _signalBus.GetStream<SignalUpdateDps>().Subscribe(x => _workshopDpsValue.text = Math.Round(x.Dps, 1).ToString()).AddTo(this);
        }
    }
}