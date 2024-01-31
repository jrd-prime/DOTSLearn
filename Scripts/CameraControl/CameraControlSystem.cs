using System;
using Jrd.UserInput;
using Jrd.Utils.Const;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.CameraControl
{
    public partial struct CameraControlSystem : ISystem
    {
        //TODO переделать
        private const float CameraSpeed = 30f;
        private const float MinFOV = 30f;
        private const float MaxFOV = 70f;
        private const float ZoomSpeed = 20f;
        private const float RotationAngle = 30f;

        private float _previousZoomDirection;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();

            EntityCommandBuffer biEcb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            EntityArchetype cameraArchetype = state.EntityManager.CreateArchetype(
                typeof(CameraData),
                typeof(MovableComponent),
                typeof(MoveDirectionData),
                typeof(ZoomDirectionData)
            );

            Entity entity = biEcb.CreateEntity(cameraArchetype);
            biEcb.SetName(entity, ENames.CameraEntityName);
            biEcb.SetComponent(entity, new MovableComponent { speed = CameraSpeed });
            biEcb.SetComponent(entity,
                new CameraData
                {
                    ZoomSpeed = ZoomSpeed,
                    MinFOV = MinFOV,
                    MaxFOV = MaxFOV,
                    RotationAngleY = RotationAngle
                });

            _previousZoomDirection = 0;
        }

        public void OnUpdate(ref SystemState state)
        {
            var instance = CameraMono.Instance;
            if (instance == null) return;

            float dt = SystemAPI.Time.DeltaTime;
            GameObject cameraHolder = instance.CameraHolder;
            Camera camera = instance.Camera;

            foreach (var aspect in SystemAPI.Query<CameraAspect>())
            {
                if (Math.Abs(aspect.ZoomDirection - _previousZoomDirection) > float.Epsilon)
                {
                    _previousZoomDirection = aspect.ZoomDirection;
                    camera.fieldOfView =
                        Mathf.Clamp(camera.fieldOfView - aspect.ZoomDirection * aspect.ZoomSpeed, aspect.MinFOV,
                            aspect.MaxFOV);
                }

                if (Equals(aspect.Direction, (float3)Vector3.zero)) continue;

                // Compensate direction.
                Vector3 direction =
                    Quaternion.AngleAxis(cameraHolder.transform.eulerAngles.y, Vector3.up) * aspect.Direction;

                // Move camera holder.
                cameraHolder.transform.position += direction * dt * aspect.Speed;

                aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero);
            }
        }
    }
}