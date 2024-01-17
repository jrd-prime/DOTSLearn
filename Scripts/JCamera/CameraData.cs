using Unity.Entities;

namespace Jrd.JCamera
{
    public struct CameraData : IComponentData
    {
        public float RotationAngleY;
        public float MinFOV;
        public float MaxFOV;
        public float ZoomSpeed;
    }
}