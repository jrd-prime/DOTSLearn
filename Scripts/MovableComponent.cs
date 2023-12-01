using Unity.Entities;

namespace DefaultNamespace
{
    public struct MovableComponent : IComponentData
    {
        public float speed;
        public bool isMoving;
    }
}