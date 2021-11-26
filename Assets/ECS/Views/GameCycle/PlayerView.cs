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
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class PlayerView : LinkableView
    {
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;

        [SerializeField] private Transform _camera;
        [SerializeField] private Transform _cameraTween;
        [SerializeField] private Transform _cameraLoseOnPathTween;
        [SerializeField] private float _movementBorderLeft = -2;
        [SerializeField] private float _movementBorderRight = 2;
        [SerializeField] private int _formationRowSize = 4;
        [SerializeField] private float _formationInRowDistance = 0.7f;
        [SerializeField] private float _formationBetweenRowDistance = 1.2f;
        [SerializeField] private Material mainPiranhaMat;

        public Transform ModelRootTransform;
        [HideInInspector] public bool IsPathComplete;

        private LinkedList<PiranhaView> _piranhas;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
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
            ModelRootTransform.localPosition = new Vector3(newX, ModelRootTransform.localPosition.y,
                ModelRootTransform.localPosition.z);
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
            piranhaView.Transform.SetParent(ModelRootTransform);
            _piranhas.AddLast(piranhaView);

            var z = Mathf.CeilToInt((float) _piranhas.Count / _formationRowSize) - 1;

            var pos = new Vector3(0, ModelRootTransform.position.y,
                ModelRootTransform.position.z - z * _formationBetweenRowDistance);

            pos.x = ModelRootTransform.position.x - (_formationRowSize % 2 == 0 ? _formationInRowDistance / 2 : 0)
                    + _formationInRowDistance * ((_piranhas.Count - z * _formationRowSize) / 2) * (_piranhas.Count %
                        2 == 0
                            ? 1
                            : -1);

            piranhaView.Transform.position = pos;

            if ((_piranhas.Count == 1 || _piranhas.Count == 2 && _formationRowSize % 2 == 0))
                piranhaView.GetComponentInChildren<SkinnedMeshRenderer>().material = mainPiranhaMat;
        }

        public int GetPiranhasCount()
        {
            return _piranhas.Count;
        }
    }
}