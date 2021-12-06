using System.Diagnostics.CodeAnalysis;
using ECS.Game.Components.Flags;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class TileView : LinkableView
    {
        [SerializeField] private Vector2Int tilePos;
        private GunCubeView _user = null;

        public ref Vector2Int GetTilePos()
        {
            return ref tilePos;
        }

        public void SetLocked(ref Material material)
        {
            GetComponent<MeshRenderer>().material = material;
            Entity.Get<TileComponent>().IsLock = true;
        }

        public bool IsAvailable(GunCubeView newUser)
        {
            return !Entity.Get<TileComponent>().IsLock && (newUser.Equals(_user) || _user == null);
        }

        public void SetUser(GunCubeView value)
        {
            _user = value;
        }

        public ref Vector2Int GetXY()
        {
            return ref tilePos;
        }
    }
}