using System;
using UnityEngine;

namespace Runtime.Game.Utils.MonoBehUtils
{
    public class ScreenVariables : MonoBehaviour
    {
        [Serializable]
        private struct Point
        {
            public Transform Transform;
            public string Key;
        }
        
        [SerializeField] private Point[] points;

        public Transform GetPoint(string key)
        {
            foreach (var point in points)
                if (key == point.Key)
                    return point.Transform;
            throw new Exception($"No position on scene with key {key} was found!");
        }
    }
}