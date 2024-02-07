using Unity.Entities;

namespace Jrd
{
    public struct FollowComponent : IComponentData
    {
        public Entity Target;
    }
}