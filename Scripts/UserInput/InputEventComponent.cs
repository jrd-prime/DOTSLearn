using Unity.Entities;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public struct InputEventComponent : IComponentData
    {
        public float3 direction;
    }
}