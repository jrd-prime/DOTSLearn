using Unity.Entities;

namespace Jrd.NeedSortComponents
{
    public struct FollowComponent : IComponentData
    {
        public Entity Target;
    }
}