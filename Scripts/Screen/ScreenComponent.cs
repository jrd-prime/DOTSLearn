﻿using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Screen
{
    public struct ScreenComponent : IComponentData
    {
        public float2 WidthAndHeight;
        public float2 ScreenCenter;
    }
}