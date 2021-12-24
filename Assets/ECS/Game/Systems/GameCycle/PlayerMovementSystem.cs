using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Game.Components.Input;
using ECS.Views.GameCycle;
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
        private readonly EcsFilter<PlayerComponent, LinkComponent, PositionComponent, SpeedComponent<PositionComponent>> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PointerDownComponent> _pointerDown;
        private readonly EcsFilter<PointerUpComponent> _pointerUp;
        private readonly EcsFilter<PointerDragComponent> _pointerDrag;
#pragma warning restore 649

        private bool _pressed;
        private Vector2 _pointerDownValue;
        private Vector2 _pointerDragValue;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            foreach (var i in _pointerDown)
            {
                _pressed = true;
                _pointerDownValue = _pointerDown.Get1(i).Position;
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
                _pointerDragValue = _pointerDrag.Get1(0).Position;
            HandlePressed();
        }

        private void HandlePressed()
        {
            foreach (var i in _player)
            {
                var playerView = _player.Get2(i).View as PlayerView;
                var movement = (_pointerDragValue - _pointerDownValue) * playerView.GetSensitivity() * _player.Get4(i).Value;
                ref var pos = ref _player.Get3(i).Value;
                var tempPos = new Vector3(
                    pos.x + Mathf.Clamp(movement.x, - playerView.GetMovementLimitX(), playerView.GetMovementLimitX())
                    , pos.y
                    , pos.z + Mathf.Clamp(movement.y, - playerView.GetMovementLimitY(), playerView.GetMovementLimitY()));
                if (!playerView.GetNavMeshAgent().CalculatePath(tempPos, new NavMeshPath()))
                    continue;
                pos = tempPos;
                playerView.GetRoot().localRotation = Quaternion.Euler(playerView.GetRoot().localRotation.x, Mathf.Atan2(movement.x,  movement.y) * 180 / Mathf.PI, playerView.GetRoot().localRotation.z);
            }
        }
    }
}