using Unity.Entities;

namespace Sources.Scripts.UserInputAndCameraControl.UserInput
{
    public struct FollowComponent : IComponentData
    {
        public Entity Target;
    }
}