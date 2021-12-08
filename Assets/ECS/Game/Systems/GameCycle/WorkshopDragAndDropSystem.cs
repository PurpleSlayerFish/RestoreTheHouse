using System;
using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Events;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.Input;
using ECS.Utils.Extensions;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class WorkshopDragAndDropSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private EcsFilter<GameStageComponent> _gameStage;
        private EcsFilter<PointerDownComponent> _pointerDown;
        private EcsFilter<PointerUpComponent> _pointerUp;
        private EcsFilter<PlayerInWorkshopComponent, LinkComponent> _player;
        private EcsFilter<HoldedComponent, PositionComponent, UIdComponent, DefaultPositionComponent> _holded;
        private EcsFilter<TileComponent> _tiles;
        private EcsFilter<TileComponent, InUseComponent> _tilesInUse;
#pragma warning restore 649

        private bool _pressed;

        private const string Cubes = "Cubes";
        private const string Tiles = "Tiles";
        private const string Background = "Background";
        private LayerMask _objectsLayerMask = LayerMask.GetMask(Cubes, Tiles);
        private LayerMask _backgroundLayerMask = LayerMask.GetMask(Background);
        private RaycastHit[] _objectsHits = new RaycastHit[4];
        private RaycastHit[] _backgroundHits = new RaycastHit[1];
        private float _tolerance = 0.00001f;

        [SuppressMessage("ReSharper", "UnusedVariable")]
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Workshop) return;
            foreach (var i in _pointerDown)
                _pressed = true;
            foreach (var i in _pointerUp)
                _pressed = false;
            if (_pressed)
            {
                if (TryCameraRaycast(ref _backgroundLayerMask, ref _backgroundHits) > 0)
                {
                    TryCameraRaycast(ref _objectsLayerMask, ref _objectsHits);
                    HandleHoldAndDrag();
                }
            }
            else
                HandleRelease();
        }

        private int TryCameraRaycast(ref LayerMask layerMask, ref RaycastHit[] hits)
        {
            ref var camera = ref (_player.Get2(0).View as PlayerInWorkshopView).GetCamera();
            return Physics.RaycastNonAlloc(camera.ScreenPointToRay(Input.mousePosition), hits, camera.farClipPlane,
                layerMask.value);
        }

        private void HandleHoldAndDrag()
        {
            for (int a = 0; a < _objectsHits.Length; a++)
            {
                if (_objectsHits[a].collider == null)
                    break;
                if (_objectsHits[a].collider.gameObject.layer == LayerMask.NameToLayer(Cubes))
                {
                    ref var gunCubeTile = ref _objectsHits[a].collider.GetComponent<GunCubeView>().GetEntity();
                    if (CheckHolded() && CheckIncorrectDeconstruction(ref gunCubeTile))
                        gunCubeTile.Get<HoldedComponent>();
                }
            }

            if (_backgroundHits[0].collider.gameObject.layer == LayerMask.NameToLayer(Background))
                foreach (var i in _holded)
                {
                    _holded.Get2(i).Value =
                        new Vector3(_backgroundHits[0].point.x, _holded.Get2(i).Value.y, _backgroundHits[0].point.z);
                    break;
                }
        }

        private bool CheckHolded()
        {
            return _holded.GetEntity(0).IsNull() ||
                   !_holded.GetEntity(0).IsNull() && !_holded.GetEntity(0).Has<HoldedComponent>();
        }

        private bool CheckIncorrectDeconstruction(ref EcsEntity gunCubeEntity)
        {
            if (!gunCubeEntity.Has<InPlaceComponent>())
                return true;
            foreach (var i in _tilesInUse)
            {
                if (_tilesInUse.Get2(i).User.Equals(gunCubeEntity.Get<UIdComponent>().Value))
                {
                    var pos = _tilesInUse.GetEntity(i).Get<TileComponent>().Position;
                    if (pos.x >= 6)
                        return true;
                    var horizontalTileEntity = _tiles.FindTile(new Vector2Int(pos.x + 1, pos.y));
                    if (NextTileNotUnderCurrentCube(ref horizontalTileEntity, ref gunCubeEntity))
                        return false;
                }
            }
            return true;
        }

        private void HandleRelease()
        {
            foreach (var i in _holded)
            {
                TryToBuild();
                _backgroundHits = new RaycastHit[_backgroundHits.Length];
                _objectsHits = new RaycastHit[_objectsHits.Length];
                _holded.GetEntity(i).Del<HoldedComponent>();
            }
        }

        private Nullable<RaycastHit> GetTileHit()
        {
            Nullable<RaycastHit> tileHit = new Nullable<RaycastHit>();
            foreach (var hit in _objectsHits)
            {
                if (hit.collider == null)
                    continue;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Tiles))
                {
                    tileHit = hit;
                    break;
                }
            }

            if (tileHit.HasValue
                && Math.Abs(_backgroundHits[0].point.x - tileHit.Value.point.x) < _tolerance
                && Math.Abs(_backgroundHits[0].point.z - tileHit.Value.point.z) < _tolerance)
                return tileHit;
            return null;
        }

        public void TryToBuild()
        {
            var hit = GetTileHit();
            if (!hit.HasValue)
            {
                SetDefaultPosition();
                return;
            }

            switch (_holded.GetEntity(0).Get<GunCubeComponent>().Type)
            {
                case EGunCubeType.ModOne:
                    HandleModOne(hit.Value);
                    break;
                case EGunCubeType.ModTwo:
                    HandleModTwo(hit.Value);
                    break;
                case EGunCubeType.MultTwo:
                    HandleMultTwo(hit.Value);
                    break;
                case EGunCubeType.MultThree:
                    HandleMultThree(hit.Value);
                    break;
                default:
                    SetDefaultPosition();
                    break;
            }
        }

        private void HandleModOne(RaycastHit hit)
        {
            ref var tileEntity = ref hit.collider.GetComponent<TileView>().GetEntity();
            if (IsAvailableTile(ref tileEntity) && CanBuildHere(ref tileEntity))
                PutGunCube(new Vector2(0, 0), tileEntity);
            else
                SetDefaultPosition();
        }

        private void HandleModTwo(RaycastHit hit)
        {
            ref var tileEntity = ref hit.collider.GetComponent<TileView>().GetEntity();
            var position = tileEntity.Get<TileComponent>().Position;
            int xModif = 0;
            Vector2 xModifVec = Vector2.zero;
            if (hit.point.x < hit.collider.transform.position.x && position.x > 1)
            {
                xModif = -1;
                xModifVec = new Vector2(-0.5f, 0);
            }

            if (hit.point.x >= hit.collider.transform.position.x && position.x < 6)
            {
                xModif = 1;
                xModifVec = new Vector2(0.5f, 0);
            }

            var secondTileEntity = _tiles.FindTile(new Vector2Int(position.x + xModif, position.y));
            if (xModif != 0 && IsAvailableAndCanBuild(ref tileEntity, ref secondTileEntity))
                PutGunCube(xModifVec, tileEntity, secondTileEntity);
            else
                SetDefaultPosition();
        }

        private void HandleMultTwo(RaycastHit hit)
        {
            ref var tileEntity = ref hit.collider.GetComponent<TileView>().GetEntity();
            var position = tileEntity.Get<TileComponent>().Position;
            int xModif = 0;
            Vector2 xModifVec = Vector2.zero;
            if (hit.point.z < hit.collider.transform.position.z && position.y > -3)
            {
                xModif = -1;
                xModifVec = new Vector2(0, -0.5f);
            }

            if (hit.point.z >= hit.collider.transform.position.z && position.y < 3)
            {
                xModif = 1;
                xModifVec = new Vector2(0, 0.5f);
            }

            var secondTileEntity = _tiles.FindTile(new Vector2Int(position.x, position.y + xModif));
            if (xModif != 0 && IsAvailableAndCanBuild(ref tileEntity, ref secondTileEntity))
                PutGunCube(xModifVec, tileEntity, secondTileEntity);
            else
                SetDefaultPosition();
        }

        private void HandleMultThree(RaycastHit hit)
        {
            ref var tileEntity = ref hit.collider.GetComponent<TileView>().GetEntity();
            var position = tileEntity.Get<TileComponent>().Position;
            if (position.y > -3 && position.y < 3)
            {
                var secondTileEntity = _tiles.FindTile(new Vector2Int(position.x, position.y + 1));
                var thirdTileEntity = _tiles.FindTile(new Vector2Int(position.x, position.y - 1));
                if (IsAvailableAndCanBuild(ref tileEntity, ref secondTileEntity, ref thirdTileEntity))
                {
                    PutGunCube(Vector2.zero, tileEntity, secondTileEntity, thirdTileEntity);
                    return;
                }
            }

            SetDefaultPosition();
        }

        private bool IsAvailableTile(ref EcsEntity tileEntity)
        {
            return !tileEntity.Get<TileComponent>().IsLock
                   && (!tileEntity.Has<InUseComponent>() ||
                       tileEntity.Get<InUseComponent>().User.Equals(_holded.Get3(0).Value));
        }

        private bool IsAvailableAndCanBuild(ref EcsEntity tileEntity1, ref EcsEntity tileEntity2)
        {
            return IsAvailableTile(ref tileEntity1) && IsAvailableTile(ref tileEntity2) &&
                   (CanBuildHere(ref tileEntity1) || CanBuildHere(ref tileEntity2));
        }

        private bool IsAvailableAndCanBuild(ref EcsEntity tileEntity1, ref EcsEntity tileEntity2,
            ref EcsEntity tileEntity3)
        {
            return IsAvailableTile(ref tileEntity1) && IsAvailableTile(ref tileEntity2) &&
                   IsAvailableTile(ref tileEntity3) && (CanBuildHere(ref tileEntity1) ||
                                                        CanBuildHere(ref tileEntity2) || CanBuildHere(ref tileEntity3));
        }

        private bool CanBuildHere(ref EcsEntity tileEntity)
        {
            ref var pos = ref tileEntity.Get<TileComponent>().Position;
            if (pos.x == 1)
                return pos.y == 0;

            var leftTileEntity = _tiles.FindTile(new Vector2Int(pos.x - 1, pos.y));
            return pos.x > 1 && NextTileNotUnderCurrentCube(ref leftTileEntity, ref _holded.GetEntity(0));
        }

        private bool NextTileNotUnderCurrentCube(ref EcsEntity tileEntity, ref EcsEntity gunCubeEntity)
        {
            return tileEntity.Has<InUseComponent>() &&
                   !tileEntity.Get<InUseComponent>().User.Equals(gunCubeEntity.Get<UIdComponent>().Value);
        }

        private void PutGunCube(Vector2 offset, params EcsEntity[] tileEntities)
        {
            ref EcsEntity gunCubeEntity = ref _holded.GetEntity(0);
            if (gunCubeEntity.Has<InPlaceComponent>())
                DropTilesInUse();
            foreach (var tileEntity in tileEntities)
            {
                tileEntity.Get<InUseComponent>().User = gunCubeEntity.Get<UIdComponent>().Value;
                tileEntity.Get<InUseComponent>().Type = gunCubeEntity.Get<GunCubeComponent>().Type;
            }
            gunCubeEntity.Get<InPlaceComponent>();
            gunCubeEntity.Get<GunCubeUpdateEventComponent>();
            gunCubeEntity.Get<PositionComponent>().Value = new Vector3(
                tileEntities[0].Get<LinkComponent>().View.Transform.position.x + offset.x
                , _holded.Get4(0).Value.y
                , tileEntities[0].Get<LinkComponent>().View.Transform.position.z + offset.y);
        }

        private void SetDefaultPosition()
        {
            _holded.Get2(0).Value = _holded.Get4(0).Value;
            if (_holded.GetEntity(0).Has<InPlaceComponent>())
                DropTilesInUse();
            _holded.GetEntity(0).Del<InPlaceComponent>();
            _holded.GetEntity(0).Get<GunCubeUpdateEventComponent>();
        }

        private void DropTilesInUse()
        {
            foreach (var i in _tilesInUse)
                if (_tilesInUse.Get2(i).User.Equals(_holded.Get3(0).Value))
                    _tilesInUse.GetEntity(i).Del<InUseComponent>();
        }
    }
}