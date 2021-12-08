using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    public class GunFireRateSystem : ReactiveSystem<GunCubeUpdateEventComponent>
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [Inject] private readonly ScreenVariables _screenVariables;

#pragma warning disable 649
        private EcsWorld _world;
        private EcsFilter<TileComponent, InUseComponent> _tilesInUse;
        private EcsFilter<ProjectileLauncherComponent> _launchers;
#pragma warning restore 649
        private readonly int width = 6, height = 7;
        private float _defaultFireRate;
        private const string GunBarrel = "GunBarrel";
        protected override bool DeleteEvent => true;
        protected override EcsFilter<GunCubeUpdateEventComponent> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            _defaultFireRate = _commonPlayerData.GetData().FireRate;
            ClearProjectileLaunchers();
            if (_tilesInUse.IsEmpty())
            {
                CreateProjectileLauncher(_defaultFireRate, _screenVariables.GetPoint(GunBarrel).position);
                return;
            }

            MatrixTile[,] matrixTiles = InitMatrixTile();
            ref MatrixTile nextTile = ref matrixTiles[0, 0];
            float currentFireRate = _defaultFireRate;
            FindAndHandleTile(ref matrixTiles, 0, 3, ref currentFireRate);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (matrixTiles[x, y] != null)
                        currentFireRate = matrixTiles[x, y].FireRate;
                    nextTile = ref matrixTiles[x, y];
                    if (nextTile == null)
                        continue;
                    if (x >= width - 1 || matrixTiles[x + 1, y] == null)
                        continue;
                    FindAndHandleTile(ref matrixTiles, x + 1, y, ref currentFireRate);
                }
            }

            UpdateGunFireRate(ref matrixTiles);
            _signalBus.Fire(new SignalUpdateDps(0));
        }

        private void UpdateGunFireRate(ref MatrixTile[,] matrixTiles)
        {
            for (int y = height - 1; y >= 0; y--)
            for (int x = width - 1; x >= 0; x--)
                if (matrixTiles[x, y] != null)
                {
                    CreateProjectileLauncher(matrixTiles[x, y].FireRate, x, y);
                    break;
                }
        }

        private void CreateProjectileLauncher(float fireRate, int x, int y)
        {
            var tile = _tilesInUse.FindTile(new Vector2Int(x + 1, y - height / 2));
            var pos = new Vector3(tile.Get<LinkComponent>().View.Transform.position.x
                                      , _world.GetEntityWithUid(tile.Get<InUseComponent>().User).Get<PositionComponent>().Value.y
                                      , tile.Get<LinkComponent>().View.Transform.position.z);
            CreateProjectileLauncher(fireRate, pos);
        }

        private void CreateProjectileLauncher(float fireRate, Vector3 position)
        {
            var entity = _world.CreateProjectileLauncher();
            entity.Get<ProjectileLauncherComponent>().FireRate = fireRate;
            entity.Get<PositionComponent>().Value = position;
        } 

        private void FindAndHandleTile(ref MatrixTile[,] matrixTiles, int x, int y, ref float fireRate)
        {
            switch (matrixTiles[x, y].Type)
            {
                case EGunCubeType.MultTwo:
                    HandleMultTwo(ref matrixTiles, x, y, ref fireRate);
                    break;
                case EGunCubeType.MultThree:
                    HandleMultThree(ref matrixTiles, x, y, ref fireRate);
                    break;
                default:
                    matrixTiles[x, y].FireRate += fireRate + _defaultFireRate;
                    break;
            }
        }

        private void HandleMultTwo(ref MatrixTile[,] matrixTiles, int x, int y, ref float fireRate)
        {
            matrixTiles[x, y].FireRate += fireRate;
            if (matrixTiles[x, y + 1] != null)
                matrixTiles[x, y + 1].FireRate += fireRate;
            else
                matrixTiles[x, y - 1].FireRate += fireRate;
        }

        private void HandleMultThree(ref MatrixTile[,] matrixTiles, int x, int y, ref float fireRate)
        {
            matrixTiles[x, y].FireRate += fireRate;
            if (matrixTiles[x, y + 1] == null)
            {
                matrixTiles[x, y - 1].FireRate += fireRate;
                matrixTiles[x, y - 2].FireRate += fireRate;
            }
            else if (matrixTiles[x, y - 1] == null)
            {
                matrixTiles[x, y + 1].FireRate += fireRate;
                matrixTiles[x, y + 2].FireRate += fireRate;
            }
            else
            {
                matrixTiles[x, y + 1].FireRate += fireRate;
                matrixTiles[x, y - 1].FireRate += fireRate;
            }
        }

        private void ClearProjectileLaunchers()
        {
            foreach (var i in _launchers)
                _launchers.GetEntity(i).Get<IsDestroyedComponent>();
        }

        private class MatrixTile
        {
            public float FireRate;
            public EGunCubeType Type;

            public MatrixTile(float fireRate, EGunCubeType type)
            {
                FireRate = fireRate;
                Type = type;
            }
        }

        private MatrixTile[,] InitMatrixTile()
        {
            MatrixTile[,] matrixTiles = new MatrixTile[width, height];
            foreach (var i in _tilesInUse)
            {
                ref var pos = ref _tilesInUse.Get1(i).Position;
                matrixTiles[pos.x - 1, pos.y + 3] =
                    new MatrixTile(0, _tilesInUse.Get2(i).Type);
            }
            return matrixTiles;
        }
    }
}