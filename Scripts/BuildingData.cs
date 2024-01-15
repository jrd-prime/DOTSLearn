using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Jrd
{
    public struct BuildingData : IComponentData
    {
        public FixedString64Bytes Name;
        public Entity Prefab;
        public FixedString64Bytes Guid;
        public float3 WorldPosition;
    }
}