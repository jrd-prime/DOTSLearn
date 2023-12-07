using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Jrd.UserInput;

namespace Jrd.JCamera
{
    public partial struct CameraControlSystem : ISystem
    {
        private const float CameraSpeed = 10f;
        private const string CameraEntityName = "_CameraEntity";

        public void OnCreate(ref SystemState state)
        {
            var em = state.EntityManager;

            // create camera archetype
            var cameraArchetype = em.CreateArchetype(
                typeof(CameraComponent),
                typeof(MovableComponent),
                typeof(MovingEventComponent),
                typeof(ZoomingEventComponent)
            );

            // create camera entity with archetype
            var cameraEntity = em.CreateEntity(cameraArchetype);
            em.SetName(cameraEntity, CameraEntityName);

            // set camera speed
            SystemAPI.SetComponent(cameraEntity, new MovableComponent { speed = CameraSpeed });
        }

        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            var instance = CameraSingleton.Instance; // camera ref
            if (instance == null) return;

            foreach (var cameraAspect in SystemAPI.Query<CameraAspect>())
            {
                var transform = instance.Camera.transform;

                // rotate the vector to compensate for camera rotation during movement
                var cameraDirection = Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * cameraAspect.Direction;

                transform.position += cameraDirection * dt * cameraAspect.Speed;
                cameraAspect.IsMoving = !Equals(cameraAspect.Direction, (float3)Vector3.zero); // set camera moving state
                instance.Camera.fieldOfView = Mathf.Clamp(instance.Camera.fieldOfView - cameraAspect.Zoom, 30, 70); // zoom
            }
        }
    }
}