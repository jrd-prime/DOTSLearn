using Unity.Entities;

namespace Jrd.JCamera
{
    public struct CameraComponent : IComponentData
    {
        public float RotationAngleY;
        public float MinFOV { get; set; }
        public float MaxFOV { get; set; }
    }
}