using System;
using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.Input;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
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
        private EcsFilter<HoldedComponent, PositionComponent, LinkComponent> _holded;
        private EcsFilter<TileComponent> _tiles;

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
                {
                    HandleHoldAndDrag();
                }
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
                    {
                        _hits[a].collider.GetComponent<GunCubeView>().SetHolded();
                    }

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
                var view = _holded.Get3(i).View as GunCubeView;
                view.TryToBuild(GetTileHit());
                ClearHits();
                view.DelHolded();
            }
        }

        private void ClearHits()
        {
            _hits = new RaycastHit[_hits.Length];
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

        private EcsComponentRef<TileComponent> FindTile(float x, float y)
        {
            foreach (var i in _tiles)
            {
                ref TileComponent tile = ref _tiles.Get1(i);
                if (tile.Position.x == x && tile.Position.y == y)
                    return _tiles.Get1Ref(i);;
            }

            throw new ArgumentOutOfRangeException();
        }

    }
}