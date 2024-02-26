using Unity.Entities;

namespace UserInputAndCameraControl.CameraControl
{
    public struct CameraData : IComponentData
    {
        public float RotationAngleY;
        public float MinFOV;
        public float MaxFOV;
        public float ZoomSpeed;
    }
}