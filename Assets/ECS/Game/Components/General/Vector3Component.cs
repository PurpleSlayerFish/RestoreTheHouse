﻿using UnityEngine;

namespace ECS.Game.Components.General
{
    public struct Vector3Component<T> where T : struct
    {
        public Vector3 Value;
    }
}