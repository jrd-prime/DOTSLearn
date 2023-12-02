using Unity.Entities;
using Unity.Mathematics;

namespace UserInput
{
    public struct InputEventComponent : IComponentData
    {
        public float3 direction;
    }
}