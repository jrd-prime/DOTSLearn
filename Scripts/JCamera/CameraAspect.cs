﻿using Unity.Entities;
using Unity.Mathematics;
using Jrd.UserInput;

namespace Jrd.JCamera
{
    public readonly partial struct CameraAspect : IAspect
    {
        public readonly Entity self;

        // private readonly RefRO<CameraComponent> _cameraComponent;
        private readonly RefRW<MovableComponent> _movableComponent;
        private readonly RefRO<InputEventComponent> _inputEventComponent;
        
        public float Speed => _movableComponent.ValueRO.speed;
        public float3 Direction => _inputEventComponent.ValueRO.direction;

        public bool IsMoving
        {
            get => _movableComponent.ValueRO.isMoving;
            set => _movableComponent.ValueRW.isMoving = value;
        }
    }
}