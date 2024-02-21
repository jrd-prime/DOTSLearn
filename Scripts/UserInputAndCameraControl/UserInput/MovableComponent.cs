using Unity.Entities;

namespace UserInputAndCameraControl.UserInput
{
    public struct MovableComponent : IComponentData
    {
        public float speed;
        public bool isMoving;
    }
}