using System.Numerics;
using Unity.Entities;
using Unity.Mathematics;

namespace DefaultNamespace
{
    public readonly partial struct CameraAspect : IAspect
    {
        public readonly Entity self;

        private readonly RefRO<CameraComponent> _cameraComponent;
        private readonly RefRO<MovableComponent> _movableComponent;
        private readonly RefRO<InputEventComponent> _inputEventComponent;
        
        public float Speed => _movableComponent.ValueRO.speed;
        public float3 Direction => _inputEventComponent.ValueRO.direction;
    }
}