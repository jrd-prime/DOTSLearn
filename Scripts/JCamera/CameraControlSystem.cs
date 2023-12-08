using System.Collections;
using Jrd.States;
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
        private const float MinFOV = 30f;
        private const float MaxFOV = 70f;

        private Entity entity;

        public void OnCreate(ref SystemState state)
        {
            var em = state.EntityManager;

            // create camera archetype
            var cameraArchetype = em.CreateArchetype(
                typeof(CameraComponent),
                typeof(MovableComponent),
                typeof(MovingEventComponent),
                typeof(ZoomingEventComponent),
                typeof(FollowComponent)
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

            var isEditModeState = false;

            foreach (var eState in SystemAPI.Query<RefRO<EditModeStateComponent>>())
            {
                isEditModeState = eState.ValueRO.State;
            }

            if (isEditModeState)
            {
                // H.T("CamSystem - EditMode");
                
                // foreach (var aspect in SystemAPI.Query<CameraAspect>())
                // {
                //     var followTargetPosition = aspect;
                //     
                //     var transform = instance.Camera.transform;
                //
                //     // rotate the vector to compensate for camera rotation during movement
                //     var cameraDirection =
                //         Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * aspect.Direction;
                //
                //     transform.position += cameraDirection * dt * aspect.Speed;
                //     aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero); // set camera moving state
                //     instance.Camera.fieldOfView =
                //         Mathf.Clamp(instance.Camera.fieldOfView - aspect.Zoom, MinFOV, MaxFOV); // zoom
                // }
                
                return;
            }

            foreach (var aspect in SystemAPI.Query<CameraAspect>())
            {
                var transform = instance.Camera.transform;

                // rotate the vector to compensate for camera rotation during movement
                var cameraDirection =
                    Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * aspect.Direction;

                transform.position += cameraDirection * dt * aspect.Speed;
                aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero); // set camera moving state
                instance.Camera.fieldOfView =
                    Mathf.Clamp(instance.Camera.fieldOfView - aspect.Zoom, MinFOV, MaxFOV); // zoom
            }
        }
    }
}