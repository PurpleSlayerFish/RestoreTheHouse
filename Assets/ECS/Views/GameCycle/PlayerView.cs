using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DG.Tweening;
using ECS.Game.Components;
using ECS.Game.Components.Input;
using ECS.Utils.Extensions;
using Ecs.Views.Linkable.Impl;
using Leopotam.Ecs;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Runtime.Signals;
using UniRx;
using UnityEngine;
using Zenject;

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
        [SerializeField] private int _formationRowSize = 4;
        [SerializeField] private float _formationInRowDistance = 0.7f;
        [SerializeField] private float _formationBetweenRowDistance = 1.2f;
        [SerializeField] private Material mainPiranhaMat;
        [SerializeField] private float _tweenMoveX = 0.2f;
        [SerializeField] private float _tweenDuration = 1f;

        public Transform RootTransform;
        public Transform NonRootTransform;
        [HideInInspector] public bool IsPathComplete;

        private LinkedList<PiranhaView> _piranhas;
        private const int Idle = 0;
        private const int Swim = -1;
        private int _stage = Idle;

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
            FinalTween();
            FinalCamera();
        }

        public void InitLevelComplete()
        {
            FinalTween();
            FinalCamera();
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

        public void HandleHorizontalMovement(Vector3 position)
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
                _piranhas.RemoveLast();
                temp.CleanUp();
            }
        }

        public void AttachPiranha(PiranhaView piranhaView)
        {
            _piranhas.AddLast(piranhaView);
            piranhaView._formationRowNumber = Mathf.CeilToInt((float) _piranhas.Count / _formationRowSize) - 1;

            var pos = new Vector3(0, RootTransform.position.y, CalculateFormationRowPos(ref piranhaView._formationRowNumber));
            pos.x = RootTransform.position.x - (_formationRowSize % 2 == 0 ? _formationInRowDistance / 2 : 0)
                    + _formationInRowDistance *
                    ((_piranhas.Count - piranhaView._formationRowNumber * _formationRowSize) / 2) * (_piranhas.Count %
                        2 == 0
                            ? 1
                            : -1);
            piranhaView.Transform.position = pos;
            
            if (_piranhas.Count <= _formationRowSize)
                piranhaView.Transform.SetParent(RootTransform);
            else
            {
                piranhaView.Transform.SetParent(NonRootTransform);
                var i = _formationRowSize;
                piranhaView.InitLeeway(GetPrev(_piranhas.Last, ref i).Value);
            }
            SetCosmetics(ref piranhaView);
        }

        private void SetCosmetics(ref PiranhaView piranhaView)
        {
            if ((_piranhas.Count == 1 || _piranhas.Count == 2 && _formationRowSize % 2 == 0))
                piranhaView.GetComponentInChildren<SkinnedMeshRenderer>().material = mainPiranhaMat;
            piranhaView.SetAnimation(_stage);
            piranhaView.Root.transform.localPosition = new Vector3(
                piranhaView.Root.transform.localPosition.x + _tweenMoveX
                , piranhaView.Root.transform.localPosition.y
                , piranhaView.Root.transform.localPosition.z);
            piranhaView.Root.DOLocalMoveX(- _tweenMoveX,  _tweenMoveX + Random.Range(-0.2f, 0.4f)).SetEase(Ease.Unset).SetLoops(-1, LoopType.Yoyo)
                .SetRelative(true);
            
        }

        private LinkedListNode<PiranhaView> GetPrev(LinkedListNode<PiranhaView> prev, ref int i)
        {
            if (i <= 0)
                return prev;
            i--;
            return GetPrev(prev.Previous, ref i);
        }
        
        public float CalculateFormationRowPos(ref int formationRowNumber)
        {
            return RootTransform.position.z - formationRowNumber * _formationBetweenRowDistance;
        }

        public int GetPiranhasCount()
        {
            return _piranhas.Count;
        }
    }
}