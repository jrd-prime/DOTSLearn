using System.Numerics;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.VisualScripting;
using UserInput;
using Vector3 = UnityEngine.Vector3;

namespace DefaultNamespace
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
                typeof(InputEventComponent)
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

            foreach (var camera in SystemAPI.Query<CameraAspect>())
            {
                instance.transform.position += (Vector3)(camera.Direction * dt * camera.Speed);
                camera.IsMoving = !Equals(camera.Direction, new float3(0, 0, 0));
            }
        }
    }
}