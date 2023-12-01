using Unity.Entities;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public struct PointComponent : IComponentData
    {
        public float3 position;
        public Entity prefab;
    }
}