using System.Diagnostics.CodeAnalysis;
using ECS.Views.Impls;
using Leopotam.Ecs;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
    public class TileView : LinkableView
    {
        [SerializeField] private Vector2Int tilePos;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
        }

        public ref Vector2Int GetTilePos()
        {
            return ref tilePos;
        }

        public void SetLockedMaterial(ref Material material)
        {
            GetComponent<MeshRenderer>().material = material;
        }
    }
}