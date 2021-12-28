using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Game.Components.Input;
using ECS.Views.GameCycle;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;
using UnityEngine.AI;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnusedVariable")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class PlayerMovementSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent, SpeedComponent<PositionComponent>>
            _player;

        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PointerDownComponent> _pointerDown;
        private readonly EcsFilter<PointerUpComponent> _pointerUp;
        private readonly EcsFilter<PointerDragComponent> _pointerDrag;
        private readonly EcsFilter<CameraComponent, LinkComponent> _camera;
#pragma warning restore 649

        private bool _pressed;
        private Vector2 _pointerDownValue;
        private Vector2 _pointerDragValue;
        private Vector2 _movement;
        private Vector3 _tempPos;
        private readonly float _cameraRotationDeg = 51f;
        private float _sin = Mathf.Sin(-51f * Mathf.Deg2Rad);
        private float _cos = Mathf.Cos(-51f * Mathf.Deg2Rad);
        private PlayerView _playerView;
        private CameraView _cameraView;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _camera)
                _cameraView = _camera.Get2(i).Get<CameraView>();

            foreach (var i in _pointerDown)
            {
                _pressed = true;
                // _pointerDownValue = _pointerDown.Get1(i).Position;
                _pointerDownValue = _cameraView.GetCamera().ScreenToViewportPoint(Input.mousePosition);
                _pointerDragValue = _pointerDownValue;
                foreach (var j in _player)
                    _player.GetEntity(j).Get<IsMovingComponent>();
            }

            foreach (var i in _pointerUp)
            {
                _pressed = false;
                foreach (var j in _player)
                    _player.GetEntity(j).Del<IsMovingComponent>();
            }

            if (!_pressed)
                return;
            foreach (var i in _pointerDrag)
                // _pointerDragValue = _pointerDrag.Get1(0).Position;
                _pointerDragValue = _cameraView.GetCamera().ScreenToViewportPoint(Input.mousePosition);
            HandlePressed();
        }

        private void HandlePressed()
        {
            foreach (var i in _player)
            {
                _playerView = _player.Get2(i).Get<PlayerView>();
                _movement = (_pointerDragValue - _pointerDownValue) * _playerView.GetSensitivity() *
                            _player.Get4(i).Value;
                ref var pos = ref _player.Get3(i).Value;
                _movement.x = Mathf.Clamp(_movement.x, -_playerView.GetMovementLimitX(),
                    _playerView.GetMovementLimitX());
                _movement.y = Mathf.Clamp(_movement.y, -_playerView.GetMovementLimitY(),
                    _playerView.GetMovementLimitY());
                _tempPos = new Vector3(
                    pos.x + _movement.x * _cos - _movement.y * _sin
                    , pos.y
                    , pos.z + _movement.x * _sin + _movement.y * _cos);

                if (!_playerView.GetNavMeshAgent().CalculatePath(_tempPos, new NavMeshPath()))
                    continue;
                pos = _tempPos;
                _playerView.GetRoot().localRotation = Quaternion.Euler(_playerView.GetRoot().localRotation.x,
                    _cameraRotationDeg + Mathf.Atan2(_movement.x, _movement.y) * 180 / Mathf.PI,
                    _playerView.GetRoot().localRotation.z);
            }
        }
    }
}