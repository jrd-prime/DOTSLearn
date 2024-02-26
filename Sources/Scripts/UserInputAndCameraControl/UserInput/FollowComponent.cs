using Unity.Entities;

namespace UserInputAndCameraControl.UserInput
{
    public struct FollowComponent : IComponentData
    {
        public Entity Target;
    }
}