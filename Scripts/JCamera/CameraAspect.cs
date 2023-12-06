using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.JCamera
{
    public readonly partial struct CameraAspect : IAspect
    {
        public readonly Entity self;

        // private readonly RefRO<CameraComponent> _cameraComponent;
        private readonly RefRW<MovableComponent> _movableComponent;
        private readonly RefRO<MovingEventComponent> _movingEventComponent;
        private readonly RefRW<ZoomingEventComponent> _zoomingEventComponent;

        public float Speed => _movableComponent.ValueRO.speed;
        public float3 Direction => _movingEventComponent.ValueRO.direction;
        public float Zoom => _zoomingEventComponent.ValueRO.zoom;

        public bool IsMoving
        {
            get => _movableComponent.ValueRO.isMoving;
            set => _movableComponent.ValueRW.isMoving = value;
        }
    }
}