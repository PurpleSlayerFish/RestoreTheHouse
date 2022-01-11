using ECS.DataSave;
using ECS.Game.Components;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Services.GameStateService;
using Services.Uid;
using Zenject;

namespace ECS.Game.Systems.Linked
{
    public class GameInitializeSystem : IEcsInitSystem
    {
        [Inject] private readonly IGameStateService<GameState> _generalState;
        
#pragma warning disable 649
        private readonly EcsWorld _world;
#pragma warning restore 649
        
        public void Init()
        {
            if (LoadGame()) return;
            CreateTimer();
            CreatePlayer();
            CreateResources();
            CreateBuildings();
            CreateReceipts();
            CreateCosts();
        }
        private bool LoadGame()
        {
            _world.NewEntity().Get<GameStageComponent>().Value = EGameStage.Play;
            var gState = _generalState.GetData();
            if (gState.SaveState.IsNullOrEmpty()) return false;
            foreach (var state in gState.SaveState)
            {
                var entity =_world.NewEntity();
                state.ReadState(entity);
            }
            return true;
        }

        private void CreateTimer()
        {
            var entity = _world.NewEntity();
            entity.Get<TimerComponent>();
            entity.Get<UIdComponent>().Value = UidGenerator.Next();
        }

        private void CreatePlayer()
        {
            _world.CreatePlayer();
        }

        private void CreateResources()
        {
            _world.CreateResources();
        }
            
        private void CreateBuildings()
        {
            _world.CreateBuildings();
        }  
        
        private void CreateReceipts()
        {
            _world.CreateReceipts();
        }

        private void CreateCosts()
        {
            _world.CreateCosts();
        }
    }
}