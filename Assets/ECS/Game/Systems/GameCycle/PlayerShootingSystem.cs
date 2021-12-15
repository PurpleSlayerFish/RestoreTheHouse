using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.Input;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class PlayerShootingSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, LinkComponent, RemapPointComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<PointerDownComponent> _pointerDown;
        private readonly EcsFilter<PointerUpComponent> _pointerUp;
        private readonly EcsFilter<PointerDragComponent> _pointerDrag;
        private readonly EcsFilter<GunComponent> _gun;
#pragma warning restore 649
        
        private bool _pressed;
        
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;
            
            foreach (var i in _pointerDown)
            {
                _pressed = true;
                foreach (var j in _gun)
                    _gun.GetEntity(j).Get<IsShootingComponent>();
            }
            foreach (var i in _pointerUp)
            {
                _pressed = false;
                foreach (var j in _gun)
                    _gun.GetEntity(j).Del<IsShootingComponent>();
            }
            if (!_pressed)
                return;
            foreach (var i in _pointerDrag)
                HandlePointerDrag(ref _pointerDrag.GetEntity(i));
        }

        private void HandlePointerDrag(ref EcsEntity entity)
        {
            var playerView = _player.Get2(0).View as PlayerView;
            var remap = _player.Get3(0);
            var newX = entity.Get<PointerDragComponent>().Position.x.Remap(
                remap.Input.x - 20, 
                remap.Input.x + 20, 
                remap.ModelPos.x + playerView.GetRotationLimitLeft(), 
                remap.ModelPos.x + playerView.GetRotationLimitRight());
            var newY = entity.Get<PointerDragComponent>().Position.y.Remap(
                remap.Input.y - 20, 
                remap.Input.y + 20, 
                remap.ModelPos.y + playerView.GetRotationLimitDown(), 
                remap.ModelPos.y + playerView.GetRotationLimitUp());
            newX = Mathf.Clamp(newX, playerView.GetRotationLimitLeft(), playerView.GetRotationLimitRight());
            newY = Mathf.Clamp(newY, playerView.GetRotationLimitDown(), playerView.GetRotationLimitUp());
            
            playerView.GetRoot().localRotation = Quaternion.Euler(/*playerView.GetRoot().localRotation.x */- newY, /*playerView.GetRoot().localRotation.*/ + newX, playerView.GetRoot().localRotation.z);
        }
    }
}