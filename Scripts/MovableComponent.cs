using Unity.Entities;

namespace Jrd
{
    public struct MovableComponent : IComponentData
    {
        public float speed;
        public bool isMoving;
    }
}