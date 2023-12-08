using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.UserInput
{
    public struct FollowComponent : IComponentData
    {
        public Entity FollowTarget;
    }
}