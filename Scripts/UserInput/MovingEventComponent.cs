using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.UserInput
{
    public struct MovingEventComponent : IComponentData
    {
        public float3 direction;
    }
}