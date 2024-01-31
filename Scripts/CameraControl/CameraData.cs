using Unity.Entities;

namespace Jrd.CameraControl
{
    public struct CameraData : IComponentData
    {
        public float RotationAngleY;
        public float MinFOV;
        public float MaxFOV;
        public float ZoomSpeed;
    }
}