using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.JCamera
{
    public readonly partial struct CameraAspect : IAspect
    {
        public readonly Entity Self;

        // private readonly RefRO<CameraComponent> _cameraComponent;
        private readonly RefRW<MovableComponent> _movableComponent;
        private readonly RefRO<MoveDirectionData> _moveDirectionData;
        private readonly RefRW<ZoomDirectionData> _zoomDirectionData;
        private readonly RefRO<CameraData> _cameraComponent;
        
        public float Speed => _movableComponent.ValueRO.speed;
        public float3 Direction => _moveDirectionData.ValueRO.Direction;
        
        public float ZoomDirection => _zoomDirectionData.ValueRO.ZoomDirection;
        public float ZoomSpeed => _cameraComponent.ValueRO.ZoomSpeed;
        public float MinFOV => _cameraComponent.ValueRO.MinFOV;
        public float MaxFOV => _cameraComponent.ValueRO.MaxFOV;
        
        public bool IsMoving
        {
            get => _movableComponent.ValueRO.isMoving;
            set => _movableComponent.ValueRW.isMoving = value;
        }

    }
}