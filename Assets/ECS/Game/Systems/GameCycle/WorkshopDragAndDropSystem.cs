using System;
using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.Input;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class WorkshopDragAndDropSystem : IEcsUpdateSystem
    {
        private EcsFilter<GameStageComponent> _gameStage;
        private EcsFilter<PointerDownComponent> _pointerDown;
        private EcsFilter<PointerUpComponent> _pointerUp;
        private EcsFilter<PlayerInWorkshopComponent, LinkComponent> _player;
        private EcsFilter<HoldedComponent, PositionComponent, UIdComponent, DefaultPositionComponent> _holded;
        private EcsFilter<TileComponent> _tiles;
        private EcsFilter<TileComponent, InUseComponent> _tilesInUse;

        private bool _pressed;
        private readonly EcsWorld _world;

        private const string Cubes = "Cubes";
        private const string Tiles = "Tiles";
        private const string Background = "Background";
        private LayerMask _layerMask = LayerMask.GetMask(Cubes, Tiles, Background);
        private RaycastHit[] _hits = new RaycastHit[4];
        private float _tolerance = 0.00001f;

        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Workshop) return;
            foreach (var i in _pointerDown)
                _pressed = true;
            foreach (var i in _pointerUp)
                _pressed = false;

            if (_pressed)
            {
                if (TryCameraRaycast() > 0)
                    HandleHoldAndDrag();
            }
            else
                HandleRelease();
        }

        private int TryCameraRaycast()
        {
            ref var camera = ref (_player.Get2(0).View as PlayerInWorkshopView).GetCamera();
            return Physics.RaycastNonAlloc(camera.ScreenPointToRay(Input.mousePosition), _hits, camera.farClipPlane,
                _layerMask.value);
        }

        private void HandleHoldAndDrag()
        {
            for (int a = 0; a < _hits.Length; a++)
            {
                if (_hits[a].collider == null)
                    break;

                if (_hits[a].collider.gameObject.layer == LayerMask.NameToLayer(Cubes))
                    if (_holded.GetEntity(0).IsNull()
                        || !_holded.GetEntity(0).IsNull() && !_holded.GetEntity(0).Has<HoldedComponent>())
                        _hits[a].collider.GetComponent<GunCubeView>().GetEntity().Get<HoldedComponent>();

                if (_hits[a].collider.gameObject.layer == LayerMask.NameToLayer(Background))
                    foreach (var i in _holded)
                    {
                        _holded.Get2(i).Value =
                            new Vector3(_hits[a].point.x, _holded.Get2(i).Value.y, _hits[a].point.z);
                        break;
                    }
            }
        }

        private void HandleRelease()
        {
            foreach (var i in _holded)
            {
                TryToBuild();
                _hits = new RaycastHit[_hits.Length];
                _holded.GetEntity(i).Del<HoldedComponent>();
            }
        }

        private Nullable<RaycastHit> GetTileHit()
        {
            Nullable<RaycastHit> backgroundHit = new Nullable<RaycastHit>();
            Nullable<RaycastHit> tileHit = new Nullable<RaycastHit>();
            foreach (var hit in _hits)
            {
                if (hit.collider == null)
                    continue;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Background))
                    backgroundHit = hit;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Tiles))
                    tileHit = hit;
            }

            if (backgroundHit.HasValue
                && tileHit.HasValue
                && Math.Abs(backgroundHit.Value.point.x - tileHit.Value.point.x) < _tolerance
                && Math.Abs(backgroundHit.Value.point.z - tileHit.Value.point.z) < _tolerance)
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
                default:
                    SetDefaultPosition();
                    break;
            }
        }

        private void HandleModOne(RaycastHit hit)
        {
            ref var tileEntity = ref hit.collider.GetComponent<TileView>().GetEntity();
            if (IsAvailableTile(ref tileEntity))
                PutGunCube(ref _holded.GetEntity(0), new Vector2(0, 0), tileEntity);
            else
                SetDefaultPosition();
        }

        private void PutGunCube(ref EcsEntity gunCubeEntity, Vector2 offset, params EcsEntity[] tileEntities)
        {
            if (gunCubeEntity.Has<OnPlaceComponent>())
                DropTilesInUse();
            foreach (var tileEntity in tileEntities)
                tileEntity.Get<InUseComponent>().User = gunCubeEntity.Get<UIdComponent>().Value;
            gunCubeEntity.Get<OnPlaceComponent>();
            gunCubeEntity.Get<PositionComponent>().Value = new Vector3(
                tileEntities[0].Get<LinkComponent>().View.Transform.position.x + offset.x
                , _holded.Get4(0).Value.y
                , tileEntities[0].Get<LinkComponent>().View.Transform.position.z + offset.y);
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
            var secondTileEntity = FindTile(new Vector2(position.x + xModif, position.y));
            if (xModif != 0 && IsAvailableTile(ref tileEntity) && IsAvailableTile(ref secondTileEntity))
                PutGunCube(ref _holded.GetEntity(0), xModifVec, tileEntity, secondTileEntity);
            else
                SetDefaultPosition();
        }

        private bool IsAvailableTile(ref EcsEntity tileEntity)
        {
            return !tileEntity.Get<TileComponent>().IsLock
                   && (!tileEntity.Has<InUseComponent>() ||
                       tileEntity.Get<InUseComponent>().User.Equals(_holded.Get3(0).Value));
        }

        private void SetDefaultPosition()
        {
            _holded.Get2(0).Value = _holded.Get4(0).Value;
            if (_holded.GetEntity(0).Has<OnPlaceComponent>())
                DropTilesInUse();
            _holded.GetEntity(0).Del<OnPlaceComponent>();
        }

        private void DropTilesInUse()
        {
            foreach (var i in _tilesInUse)
                if (_tilesInUse.Get2(i).User.Equals(_holded.Get3(0).Value))
                    _tilesInUse.GetEntity(i).Del<InUseComponent>();
        }

        private EcsEntity FindTile(Vector2 position)
        {
            foreach (var i in _tiles)
            {
                // ref TileComponent tile = ref _tiles.Get1(i);
                ref TileComponent tile = ref _tiles.GetEntity(i).Get<TileComponent>();
                if (tile.Position.x == position.x && tile.Position.y == position.y)
                    return _tiles.GetEntity(i);
            }

            throw new ArgumentOutOfRangeException();
        }
    }
}