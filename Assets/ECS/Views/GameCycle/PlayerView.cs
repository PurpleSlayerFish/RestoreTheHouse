using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Input;
using ECS.Utils.Extensions;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Runtime.Signals;
using UniRx;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PlayerView : LinkableView
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [HideInInspector] public bool IsPathComplete;
        
        [SerializeField] private float _speed = 6f;
        private const int Idle = 0;
        private int _stage = Idle;
        private float _animationOffset = 0;
        private float _slowedSpeed;
        private float _currentSpeed;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            // _signalBus.GetStream<SignalPlayerAnimation>().Subscribe(_ =>
            // {
            //     _stage = Swim;
            //     foreach (var piranhaView in _piranhas)
            //         piranhaView.SetAnimation(_stage);
            // }).AddTo(this);
            // entity.Get<ImpactComponent>().Value = _commonPlayerData.GetData().PiranhasProgression;
            // _piranhas = new LinkedList<PiranhaView>();
            _slowedSpeed = _speed * 0.75f;
            _currentSpeed = _speed;
        }

        public void InitLevelLose()
        {
        }

        public void InitLevelComplete()
        {
        }

        private float GetRandonPlusMinus()
        {
            return Random.Range(0, 2) == 0 ? 1 : -1;
        }

        public void RestoreSpeed()
        {
            _currentSpeed = _speed;
        }

        public ref float GetCurrentSpeed()
        {
            return ref _currentSpeed ;
        }
    }
}