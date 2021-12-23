using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class MarketView : LinkableView
    {
        [SerializeField] private Transform _salePoint;
        [SerializeField] private Transform _clearPoint;

        public Vector3 GetSalePointPos()
        {
            return _salePoint.position;
        }
        
        public Vector3 GetClearPointPos()
        {
            return _clearPoint.position;
        }
    }
}