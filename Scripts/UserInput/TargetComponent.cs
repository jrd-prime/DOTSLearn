using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jrd.UserInput
{
    public struct TargetComponent : IComponentData
    {
        public float3 TargetPosition;
    }
}