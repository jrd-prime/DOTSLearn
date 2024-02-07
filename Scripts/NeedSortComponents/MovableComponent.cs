using Unity.Entities;

namespace Jrd.NeedSortComponents
{
    public struct MovableComponent : IComponentData
    {
        public float speed;
        public bool isMoving;
    }
}