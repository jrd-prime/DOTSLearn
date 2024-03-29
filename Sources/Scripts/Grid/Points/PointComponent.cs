﻿using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.Grid.Points
{
    public struct PointComponent : IComponentData
    {
        public Entity self;
        public int id;
        public float3 pointPosition;
        public bool isBlocked;
        public Entity entityOnPoint;
        public Entity prefab;
    }
}