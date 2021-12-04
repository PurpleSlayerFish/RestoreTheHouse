using System.Diagnostics.CodeAnalysis;
using DataBase.Game;
using ECS.Core.Utils.SystemInterfaces;
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
    public class WorkshopDragAndDropSystem : IEcsUpdateSystem
    {
        private EcsFilter<GameStageComponent> _gameStage;
        private EcsFilter<PointerDownComponent> _pointerDown;
        private EcsFilter<PointerUpComponent> _pointerUp;
        private EcsFilter<PlayerInWorkshopComponent, LinkComponent> _player;

        private bool _pressed;
        private readonly EcsWorld _world;

        // private const string RaycastMaskName = "RaycastPlane";
        private const string Cubes = "Cubes";
        private const string Tiles = "Tiles";
        private const string Background = "Background";
        private LayerMask _layerMask = LayerMask.GetMask(Cubes, Tiles, Background);
        private RaycastHit[] _hits = new RaycastHit[3];

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
            foreach (var hit in _hits)
            {
                if (hit.collider == null)
                    break;
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Cubes))
                {
                    foreach (var i in _player)
                    {
                        _player.GetEntity(i).Get<HoldedComponent>().Target = hit.collider.GetComponent<GunCubeView>();
                        // .UpdatePosition();
                    }

                    // hit.collider.GetComponent<GunCubeView>().GetUid();
                }

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Background))
                {
                    foreach (var i in _player)
                    {
                        if (_player.GetEntity(i).Has<HoldedComponent>())
                        {
                            (_player.GetEntity(i).Get<HoldedComponent>().Target as GunCubeView).UpdatePosition(
                                hit.point.x, hit.point.z);
                        }
                    }
                }

                Debug.Log(hit.collider + ": " + hit.point);
            }
        }

        private void HandleRelease()
        {
            foreach (var i in _player)
            {
                if (_player.GetEntity(i).Has<HoldedComponent>())
                {
                    _player.GetEntity(i).Del<HoldedComponent>();
                    ClearHits();
                }
            }
        }

        private void ClearHits()
        {
            _hits = new RaycastHit[_hits.Length];
        }
    }
}