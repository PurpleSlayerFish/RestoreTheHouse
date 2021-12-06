using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class AddImpactToPlayerSystem : ReactiveSystem<AddImpactEventComponent>
    {
        [Inject] private readonly SignalBus _signalBus;
        private readonly EcsFilter<PlayerComponent, ImpactComponent, LinkComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private EcsWorld _world;

        protected override bool DeleteEvent => false;
        protected override EcsFilter<AddImpactEventComponent> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            var playerView = _player.Get3(0).View as PlayerView;

            ref var playerImpact = ref _player.Get2(0).Value;
            ref var impact = ref entity.Get<ImpactComponent>().Value;

            // switch (entity.Get<ImpactTypeComponent>().Value)
            // {
            //     case EGunCubeType.Addition:
            //         playerImpact += impact;
            //         break;
            //
            //     case EGunCubeType.Subtraction:
            //         playerImpact -= impact;
            //         break;
            //
            //     case EGunCubeType.Multiplication:
            //         playerImpact *= impact;
            //         break;
            //
            //     case EGunCubeType.Division:
            //         playerImpact = Mathf.CeilToInt((float) playerImpact / impact);
            //         break;
            //     default:
            //         throw new InvalidEnumArgumentException();
            // }
            
            _signalBus.Fire(new SignalUpdateImpact(playerImpact));

            entity.Destroy();
        }
    }
}