using Unity.Entities;

namespace Sources.Scripts.UserInputAndCameraControl.UserInput
{
    public struct MovableComponent : IComponentData
    {
        public float speed;
        public bool isMoving;
    }
}