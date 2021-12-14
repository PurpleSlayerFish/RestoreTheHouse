using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;

namespace ECS.Game.Systems.GameCycle
{
    public class ChestTrialSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, InTrialComponent> _player;
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<ChestComponent, HealthPointComponent, LinkComponent> _chest;
#pragma warning restore 649

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void Run()
        {
            foreach (var i in _player)
            {
                foreach (var j in _chest)
                {
                    var chestView = (_chest.Get3(j).View as ChestView);
                    if (_player.GetEntity(i).Get<ElapsedTimeComponent>().Value >= chestView.GetTrialTime())
                    {
                        if (_chest.Get2(i).Value <= 0)
                            _chest.GetEntity(j).Get<AddImpactEventComponent>();
                        _player.GetEntity(i).Del<InTrialComponent>();
                        foreach (var k in _gameStage)
                            _gameStage.GetEntity(k).Get<ChangeStageComponent>().Value = EGameStage.Complete;
                    }
                }
            }
        }
    }
}