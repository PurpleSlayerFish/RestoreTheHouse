using System;
using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Utils.Extensions;
using Leopotam.Ecs;
using PdUtils;
using Runtime.DataBase.Game;
using Runtime.Game.Utils.MonoBehUtils;
using Runtime.Services.CommonPlayerData;
using Runtime.Services.CommonPlayerData.Data;
using Runtime.Signals;
using UnityEngine;
using Zenject;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    public class GunFireRateSystem : ReactiveSystem<GunCubeUpdateEventComponent>
    {
        [Inject] private readonly SignalBus _signalBus;
        [Inject] private readonly ICommonPlayerDataService<CommonPlayerData> _commonPlayerData;
        [Inject] private readonly ScreenVariables _screenVariables;

#pragma warning disable 649
        private EcsWorld _world;
        private EcsFilter<TileComponent, InUseComponent> _tilesInUse;
        private EcsFilter<ProjectileLauncherComponent> _launchers;
        private EcsFilter<GunComponent> _gun;
#pragma warning restore 649
        private readonly int width = 6;
        private readonly int height = 7;
        private readonly int halfHeight = 3;
        private float _defaultFireRate;
        private float _totalFireRate;
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
                UpdateFireRateUi();
                return;
            }

            MatrixTile[,] matrixTiles = InitMatrixTile();
            ref MatrixTile nextTile = ref matrixTiles[0, 0];
            float currentFireRate = _defaultFireRate;
            FindAndHandleTile(ref matrixTiles, 0, halfHeight, ref currentFireRate);
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

            UpdateGunBarrelsFireRate(ref matrixTiles);
            UpdateFireRateUi();
        }

        private void UpdateGunBarrelsFireRate(ref MatrixTile[,] matrixTiles)
        {
            for (int y = height - 1; y >= 0; y--)
            for (int x = width - 1; x >= 0; x--)
                if (matrixTiles[x, y] != null)
                {
                    CreateProjectileLauncher(matrixTiles[x, y].FireRate, x, y);
                    break;
                }
        }

        private void UpdateFireRateUi()
        {
            foreach (var i in _gun)
            {
                _gun.Get1(i).TotalFireRate = _totalFireRate == 0f ? _defaultFireRate : _totalFireRate;
                _signalBus.Fire(new SignalUpdateDps(_totalFireRate));
            }
        }

        private void CreateProjectileLauncher(float fireRate, int x, int y)
        {
            var tile = _tilesInUse.FindTile(new Vector2Int(x + 1, y - halfHeight));
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
            _totalFireRate = (float) Math.Round(_totalFireRate + fireRate, 1);
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
                    AddFireRate(ref matrixTiles[x, y], fireRate + _defaultFireRate);
                    break;
            }
        }

        private void HandleMultTwo(ref MatrixTile[,] matrixTiles, int x, int y, ref float fireRate)
        {
            AddFireRate(ref matrixTiles[x, y], fireRate);
            if (y < height - 1 && matrixTiles[x, y + 1] != null && matrixTiles[x, y + 1].User.Equals(matrixTiles[x, y].User))
                AddFireRate(ref matrixTiles[x, y + 1], fireRate);
            if (y > 0 && matrixTiles[x, y - 1] != null && matrixTiles[x, y - 1].User.Equals(matrixTiles[x, y].User))
                AddFireRate(ref matrixTiles[x, y - 1], fireRate);
        }

        private void HandleMultThree(ref MatrixTile[,] matrixTiles, int x, int y, ref float fireRate)
        {
            AddFireRate(ref matrixTiles[x, y], fireRate);
            if (y < height - 1 && matrixTiles[x, y + 1] != null && matrixTiles[x, y + 1].User.Equals(matrixTiles[x, y].User))
                AddFireRate(ref matrixTiles[x, y + 1], fireRate);
            if (y < height - 2 && matrixTiles[x, y + 2] != null && matrixTiles[x, y + 2].User.Equals(matrixTiles[x, y].User))
                AddFireRate(ref matrixTiles[x, y + 2], fireRate);
            if (y > 0 && matrixTiles[x, y - 1] != null && matrixTiles[x, y - 1].User.Equals(matrixTiles[x, y].User))
                AddFireRate(ref matrixTiles[x, y - 1], fireRate);
            if (y > 1 && matrixTiles[x, y - 2] != null && matrixTiles[x, y - 2].User.Equals(matrixTiles[x, y].User))
                AddFireRate(ref matrixTiles[x, y - 2], fireRate);
        }

        private void AddFireRate(ref MatrixTile matrixTile, float fireRate)
        {
            matrixTile.FireRate = (float) Math.Round(matrixTile.FireRate + fireRate, 1);
        }

        private void ClearProjectileLaunchers()
        {
            _totalFireRate = 0;
            foreach (var i in _launchers)
                _launchers.GetEntity(i).Get<IsDestroyedComponent>();
        }

        private class MatrixTile
        {
            public float FireRate;
            public EGunCubeType Type;
            public Uid User;

            public MatrixTile(float fireRate, EGunCubeType type, ref Uid user)
            {
                FireRate = fireRate;
                Type = type;
                User = user;
            }
        }

        private MatrixTile[,] InitMatrixTile()
        {
            MatrixTile[,] matrixTiles = new MatrixTile[width, height];
            foreach (var i in _tilesInUse)
            {
                ref var pos = ref _tilesInUse.Get1(i).Position;
                matrixTiles[pos.x - 1, pos.y + 3] =
                    new MatrixTile(0, _tilesInUse.Get2(i).Type, ref _tilesInUse.Get2(i).User);
            }
            return matrixTiles;
        }
    }
}