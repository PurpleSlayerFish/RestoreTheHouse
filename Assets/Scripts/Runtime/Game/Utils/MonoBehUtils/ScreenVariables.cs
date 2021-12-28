using System;
using UnityEngine;

namespace Runtime.Game.Utils.MonoBehUtils
{
    public class ScreenVariables : MonoBehaviour
    {
        [SerializeField] private TransformPoint[] _points;
        [SerializeField] private FloatValue[] _values;

        [Serializable]
        private struct TransformPoint
        {
            public string Key;
            public Transform Transform;
        }
        
        [Serializable]
        private struct FloatValue
        {
            public string Key;
            public float Value;
        }
        
        public Transform GetTransformPoint(string key)
        {
            foreach (var point in _points)
                if (key == point.Key)
                    return point.Transform;
            throw new Exception($"No position on scene with key {key} was found!");
        }
        
        public float GetFloatValue(string key)
        {
            foreach (var value in _values)
                if (key == value.Key)
                    return value.Value;
            throw new Exception($"No position on scene with key {key} was found!");
        }
    }
}