using System;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Views.Impls;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class GunCubeView : LinkableView
    {
        [SerializeField] private EGunCubeType _gunCubeType;
        private Vector3 _defaultPosition;
        private WorkshopView _workshopView;
        private TileView[] _tilesInUse;
        private bool _isTiled;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            _defaultPosition = transform.position;
            _workshopView = FindObjectOfType<WorkshopView>();
            InitTileInUse();
            _isTiled = false;
        }

        private void InitTileInUse()
        {
            switch (_gunCubeType)
            {
                case EGunCubeType.ModOne:
                    _tilesInUse = new TileView[1];
                    break;
                case EGunCubeType.ModTwo:
                    _tilesInUse = new TileView[2];
                    break;
                default:
                    _tilesInUse = new TileView[1];
                    break;
            }
        }

        public void TryToBuild(Nullable<RaycastHit> hit)
        {
            if (!hit.HasValue)
            {
                SetDefaultPosition();
                return;
            }

            switch (_gunCubeType)
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
            var tileView = hit.collider.GetComponent<TileView>();
            if (tileView.IsAvailable(this))
            {
                if (_isTiled)
                    DropTilesInUse();
                _tilesInUse[0] = tileView;
                tileView.SetUser(this);
                _isTiled = true;
                Entity.Get<PositionComponent>().Value = SetTilePos(tileView.Transform.position);
            }
            else
                SetDefaultPosition();
        }

        private void HandleModTwo(RaycastHit hit)
        {
            var raycastedTileView = hit.collider.GetComponent<TileView>();
            if (hit.point.x < raycastedTileView.Transform.position.x)
            {
                if (raycastedTileView.GetXY().x <= 0)
                    SetDefaultPosition();
                // if (!_workshopView.GetTile(raycastedTileView.GetXY().x, raycastedTileView.GetXY().y).IsAvailable())
                    // return false;
                // if (_workshopView.GetTile(raycastedTileView.GetXY().x - 1, raycastedTileView.GetXY().y).IsAvailable())
                    // return true;
            }
            
            SetDefaultPosition();
        }

        private void SetDefaultPosition()
        {
            Entity.Get<PositionComponent>().Value = _defaultPosition;
            if (_isTiled)
                DropTilesInUse();
            _isTiled = false;
        }

        private void DropTilesInUse()
        {
            foreach (var tileInUse in _tilesInUse)
                tileInUse.SetUser(null);
        }

        private Vector3 SetTilePos(Vector3 position)
        {
            return new Vector3(position.x, _defaultPosition.y, position.z);
        }

        public void SetHolded()
        {
            Entity.Get<HoldedComponent>();
        }
        
        public void DelHolded()
        {
            Entity.Del<HoldedComponent>();
        }

    }
}