using Jrd.GameStates.BuildingState.TempBuilding;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Jrd.UserInput;
using Unity.Transforms;

namespace Jrd.JCamera
{
    public partial struct CameraControlSystem : ISystem
    {
        private const float CameraSpeed = 10f;
        private const string CameraEntityName = "___ Main Camera Entity";
        private const float MinFOV = 30f;
        private const float MaxFOV = 70f;

        private Entity _entity;

        public void OnCreate(ref SystemState state)
        {
            var em = state.EntityManager;

            // create camera archetype
            var cameraArchetype = em.CreateArchetype(
                typeof(CameraComponent),
                typeof(MovableComponent),
                typeof(MovingEventComponent),
                typeof(ZoomingEventComponent),
                typeof(FollowComponent),
                typeof(MoveByPlayerTag),
                typeof(LocalTransform)
            );

            // create camera entity with archetype
            _entity = em.CreateEntity(cameraArchetype);
            em.SetName(_entity, CameraEntityName);

            // set camera speed
            SystemAPI.SetComponent(_entity, new MovableComponent { speed = CameraSpeed });
        }

        public void OnUpdate(ref SystemState state)
        {
            var dt = SystemAPI.Time.DeltaTime;
            var instance = CameraSingleton.Instance; // camera ref

            if (instance == null) return;

            state.EntityManager.SetComponentData(_entity, new CameraComponent
            {
                RotationAngleY = instance.Camera.transform.eulerAngles.y
            });


            foreach (var aspect in SystemAPI.Query<CameraAspect>())
            {
                Debug.Log("Moved by player " + this);
                // var followTargetPosition = aspect;
                //
                // var transform = instance.Camera.transform;
                //
                // // rotate the vector to compensate for camera rotation during movement
                // var cameraDirection =
                //     Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) * aspect.Direction;
                //
                // transform.position += cameraDirection * dt * aspect.Speed;
                // aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero); // set camera moving state
                // instance.Camera.fieldOfView =
                //     Mathf.Clamp(instance.Camera.fieldOfView - aspect.Zoom, MinFOV, MaxFOV); // zoom

                // var transform = instance.Camera.transform;
                //
                // Debug.LogWarning(" transform.position " +  transform.position);
                // // rotate the vector to compensate for camera rotation during movement
                // var cameraDirection =
                //     Quaternion.AngleAxis(transform.eulerAngles.y, Vector3.up) *
                //     aspect.Direction;
                //
                // Debug.LogWarning("camera dir " + cameraDirection);
                //
                //
                // // transform.position += (Vector3)cameraDirection * dt * aspect.Speed;
                //
                // aspect.IsMoving = !Equals(aspect.Direction, (float3)Vector3.zero); // set camera moving state
                //
                // instance.Camera.fieldOfView =
                //     Mathf.Clamp(instance.Camera.fieldOfView - aspect.Zoom, MinFOV, MaxFOV); // zoom
            }
        }
    }
}