﻿using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Build
{
    public struct BuildingDetailsComponent : IComponentData
    {
        public Entity prefab;
        public float3 position;
        public int id;
        public FixedBytes62 name;
        public Entity entity;
    }
}