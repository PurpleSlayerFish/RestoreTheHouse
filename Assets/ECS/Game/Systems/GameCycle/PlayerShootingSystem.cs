using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
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
    public class PlayerShootingSystem : ReactiveSystem<PointerDragComponent>
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, LinkComponent, RemapPointComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
#pragma warning restore 649
        
        private float _limitRotationUp = 30f;
        private float _limitRotationDown = -20f;
        private float _limitRotationRight = 25f;
        private float _limitRotationLeft = -25f;

        protected override EcsFilter<PointerDragComponent> ReactiveFilter { get; }

        
        protected override void Execute(EcsEntity entity)
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;

            var playerView = _player.Get2(0).View as PlayerView;
                
            // _player.Get3(0).ModelPos = playerView.GetRoot().localPosition;
            var remap = _player.Get3(0);
            var newX = entity.Get<PointerDragComponent>().Position.x.Remap(
                remap.Input.x - 20, 
                remap.Input.x + 20, 
                remap.ModelPos.x + playerView.GetRotationLimitLeft(), 
                remap.ModelPos.x + playerView.GetRotationLimitRight());
            newX = Mathf.Clamp(newX, playerView.GetRotationLimitLeft(), playerView.GetRotationLimitRight());
            
            var newY = entity.Get<PointerDragComponent>().Position.y.Remap(
                remap.Input.y - 20, 
                remap.Input.y + 20, 
                remap.ModelPos.y + playerView.GetRotationLimitDown(), 
                remap.ModelPos.y + playerView.GetRotationLimitUp());
            newY = Mathf.Clamp(newY, playerView.GetRotationLimitDown(), playerView.GetRotationLimitUp());
            
            playerView.GetRoot().rotation = Quaternion.Euler(playerView.GetRoot().rotation.x - newY, playerView.GetRoot().rotation.y + newX, playerView.GetRoot().rotation.z);
        }
    }
}