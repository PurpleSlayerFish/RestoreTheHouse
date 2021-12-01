using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Input;
using ECS.Utils.Extensions;
using Ecs.Views.Linkable.Impl;
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
        [Inject] private SignalBus _signalBus;

        [SerializeField] private Transform _camera;
        [SerializeField] private Transform _cameraTween;
        [SerializeField] private Transform _cameraLoseOnPathTween;
        [SerializeField] private float _movementBorderLeft = -2;
        [SerializeField] private float _movementBorderRight = 2;
        [SerializeField] private float _speed = 6f;
        [SerializeField] private int _formationRowSize = 4;
        [SerializeField] private float _formationInRowDistance = 0.7f;
        [SerializeField] private float _formationBetweenRowDistance = 1.2f;
        [SerializeField] private Material mainPiranhaMat;
        [SerializeField] private float _tweenMove = 0.4f;
        [SerializeField] private float _tweenDurationFrom = 0.5f;
        [SerializeField] private float _tweenDurationTo = 1f;
        [SerializeField] private Vector3 _spawner1;
        [SerializeField] private Vector3 _spawner2;
        [SerializeField] private float _fromSpawnDuration = 0.7f;

        public Transform RootTransform;
        [SerializeField] private Transform _nonRootTransform;
        [HideInInspector] public bool IsPathComplete;

        private LinkedList<PiranhaView> _piranhas;
        private const int Idle = 0;
        private const int Swim = -1;
        private const int Dead = -2;
        private const int Bite = -3;
        private const float BiteOffset = -0.26f;
        private const int Attack1 = -4;
        private const float Attack1Offset = 0;
        private const int Attack2 = -5;
        private const float Attack2Offset = -0.11f;
        private const int JumpInPlace = -6;
        private const float JumpInPlaceOffset = -0.26f;
        private int _stage = Idle;
        private float _animationOffset = 0;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _signalBus.GetStream<SignalPlayerAnimation>().Subscribe(_ =>
            {
                _stage = Swim;
                foreach (var piranhaView in _piranhas)
                    piranhaView.SetAnimation(_stage);
            }).AddTo(this);
            entity.Get<ImpactComponent>().Value = _commonPlayerData.GetData().PiranhasProgression;
            _piranhas = new LinkedList<PiranhaView>();
        }

        public void InitLevelLose()
        {
            // FinalTween();
        }

        public void InitLevelComplete()
        {
            // FinalTween();
            var points = FindObjectsOfType<FinishPoint>();
            foreach (var piranhaView in _piranhas)
            {
                var randomPoint = points[Random.Range(0, points.Length)];
                var randomAnimation = Random.Range(JumpInPlace, Bite + 1);
                SetAnimationOffset(randomAnimation);
                piranhaView.KillLeeway();
                piranhaView.TweenToMan(GetPointOnCircle(randomPoint, Random.Range(180f, 360f)))
                    .OnComplete(() =>
                        {
                            piranhaView.Transform.LookAt(randomPoint.transform.position);
                            piranhaView.SetAnimation(Random.Range(JumpInPlace, Bite + 1));
                        });
            }
        }
        
        private void SetAnimationOffset(int stage)
        {
            if (stage == Attack1)
                _animationOffset = Attack1Offset;
            if (stage == Attack2)
                _animationOffset = Attack2Offset;
            if (stage == Bite)
                _animationOffset = BiteOffset;
            if (stage == JumpInPlace)
                _animationOffset = JumpInPlaceOffset;
        }

        private Vector3 GetPointOnCircle(FinishPoint point, float randomRotate)
        {
            return new Vector3(
                Mathf.Cos(Mathf.Deg2Rad * randomRotate) * (point.Radius + _animationOffset) + point.transform.position.x,
                point.transform.position.y,
                Mathf.Sin(Mathf.Deg2Rad * randomRotate) * (point.Radius + _animationOffset) + point.transform.position.z);
        }

        private void FinalCamera()
        {
            if (IsPathComplete)
            {
                _camera.DOLocalMove(_cameraTween.localPosition, 0.5f);
                _camera.DOLocalRotate(_cameraTween.localRotation.eulerAngles, 0.5f);
            }
            else
            {
                _camera.DOLocalMove(_cameraLoseOnPathTween.localPosition, 0.5f);
                _camera.DOLocalRotate(_cameraLoseOnPathTween.localRotation.eulerAngles, 0.5f);
            }
        }

        private void FinalTween()
        {
            // ModelRootTransform.DOLocalMove(Vector3.zero, 0.3f);
        }

        public void HandleHorizontalMovement(ref Vector3 position)
        {
            var remap = Entity.Get<RemapPointComponent>();
            var newX = position.x.Remap(
                remap.Input.x - 20,
                remap.Input.x + 20,
                remap.ModelPos.x + _movementBorderLeft,
                remap.ModelPos.x + _movementBorderRight);

            newX = Mathf.Clamp(newX, _movementBorderLeft, _movementBorderRight);
            RootTransform.localPosition = new Vector3(newX, RootTransform.localPosition.y,
                RootTransform.localPosition.z);
        }

        public void DestroyPirahnas(int count)
        {
            PiranhaView temp;
            var result = count < 0 ? _piranhas.Count : count;
            for (int i = 0; i < result; i++)
            {
                temp = _piranhas.Last.Value;
                temp.Transform.SetParent(null);
                temp.KillLeeway();
                temp.SetAnimation(Dead);
                temp.InitPeaceDead();
                _piranhas.RemoveLast();
            }
        }

        public void AttachPiranha(PiranhaView piranhaView)
        {
            _piranhas.AddLast(piranhaView);
            piranhaView._formationRowNumber = Mathf.CeilToInt((float) _piranhas.Count / _formationRowSize) - 1;
            piranhaView.Transform.position = GetPiranhaTweenPosition(ref piranhaView);
            if (_piranhas.Count <= _formationRowSize)
                piranhaView.Transform.SetParent(RootTransform);
            else
            {
                piranhaView.Transform.SetParent(_nonRootTransform);
                var i = _formationRowSize;
                piranhaView.InitLeeway(GetPrev(_piranhas.Last, ref i).Value);
            }
            if ((_piranhas.Count == 1 || _piranhas.Count == 2 && _formationRowSize % 2 == 0))
                piranhaView.GetComponentInChildren<SkinnedMeshRenderer>().material = mainPiranhaMat;
            piranhaView.TweenFromSpawn(GetRandomizedSpawner(), ref _fromSpawnDuration, ref _tweenMove)
                .OnStepComplete(() => piranhaView.InitTween(ref _tweenMove, ref _tweenDurationFrom,ref _tweenDurationTo));
            piranhaView.SetAnimation(_stage);
        }

        private Vector3 GetPiranhaTweenPosition(ref PiranhaView piranhaView)
        {
            var pos = new Vector3(0, RootTransform.position.y,
                CalculateFormationRowPos(ref piranhaView._formationRowNumber));
            pos.x = RootTransform.position.x - (_formationRowSize % 2 == 0 ? _formationInRowDistance / 2 : 0)
                    + _formationInRowDistance *
                    ((_piranhas.Count - piranhaView._formationRowNumber * _formationRowSize) / 2) * (_piranhas.Count %
                        2 == 0
                            ? 1
                            : -1);
            return pos;
        }

        private LinkedListNode<PiranhaView> GetPrev(LinkedListNode<PiranhaView> prev, ref int i)
        {
            if (i <= 0)
                return prev;
            i--;
            return GetPrev(prev.Previous, ref i);
        }

        private Vector3 GetRandomizedSpawner()
        {
            return new Vector3(
                Random.Range(_spawner1.x, _spawner2.x) * GetRandonPlusMinus()
                , Random.Range(_spawner1.y, _spawner2.y)
            ,Random.Range(_spawner1.z, _spawner2.z));

        }

        private float GetRandonPlusMinus()
        {
            return Random.Range(0, 2) == 0 ? 1 : -1;
        }
        
        public float CalculateFormationRowPos(ref int formationRowNumber)
        {
            return RootTransform.position.z - formationRowNumber * _formationBetweenRowDistance;
        }

        public int GetPiranhasCount()
        {
            return _piranhas.Count;
        }
        
        
        public void PiranhasUncheck()
        {
            foreach (var piranhaView in _piranhas)
                piranhaView.EatCheck = false;
        }

        public void EatPiranha(ref SharkView sharkView)
        {
            var piranhaView = _piranhas.Last.Value;
            if (piranhaView.EatCheck)
                return;
            piranhaView.EatCheck = true;
            piranhaView.KillLeeway();
            piranhaView.TweenToShark(sharkView).OnComplete(() => piranhaView.CleanUp());
            _piranhas.RemoveLast();
            Entity.Get<ImpactComponent>().Value--;
            _signalBus.Fire(new SignalUpdateImpact(Entity.Get<ImpactComponent>().Value));
        }

        public ref float GetSpeed()
        {
            return ref _speed ;
        }
    }
}