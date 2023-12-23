using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Jrd.UserInput;
using Jrd.Utils.Const;
using Unity.Collections;

namespace Jrd.JCamera
{
    public partial struct CameraMovementSystem : ISystem
    {
        private const float CameraSpeed = 10f;
        private const float MinFOV = 30f;
        private const float MaxFOV = 70f;
        private const float RotationAngle = 30f;

        public void OnCreate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var em = state.EntityManager;


            var cameraArchetype = em.CreateArchetype(
                typeof(CameraComponent),
                typeof(MovableComponent),
                typeof(MovingEventComponent),
                typeof(ZoomingEventComponent)
            );

            var entity = ecb.CreateEntity(cameraArchetype);
            ecb.SetName(entity, ENames.CameraEntityName);
            ecb.SetComponent(entity, new MovableComponent { speed = CameraSpeed });
            ecb.SetComponent(entity,
                new CameraComponent
                {
                    MinFOV = MinFOV,
                    MaxFOV = MaxFOV,
                    RotationAngleY = RotationAngle
                });


            ecb.Playback(em);
            ecb.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            var instance = CameraMono.Instance;
            if (instance == null) return;

            var dt = SystemAPI.Time.DeltaTime;
            var cameraHolder = instance.CameraHolder;

            foreach (var aspect in SystemAPI.Query<CameraAspect>())
            {
                if (Equals(aspect.Direction, (float3)Vector3.zero)) continue;

                // Debug.Log("// default movement");
                // compensate direction
                var direction =
                    Quaternion.AngleAxis(cameraHolder.transform.eulerAngles.y, Vector3.up) * aspect.Direction;

                cameraHolder.transform.position += direction * dt * aspect.Speed;

                aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero);
            }
        }
    }
}