using System;
using Game.Utils;
using Game.Utils.MonoBehUtils;
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
        [SerializeField] private QuadArea _quadArea;
        [SerializeField] private QuadArea _saveArea;

        [SerializeField] private Point[] points;

        public Transform GetPoint(string key)
        {
            foreach (var point in points)
                if (key == point.Key)
                    return point.Transform;
            throw new Exception($"No position on scene with key {key} was found!");
        }

        public Vector3 GetRandomPoint() => LevelUtils.CalculateRandomPosition(_quadArea.point1.position, _quadArea.point2.position);
        public QuadArea GetSaveArea => _saveArea;

    }
}