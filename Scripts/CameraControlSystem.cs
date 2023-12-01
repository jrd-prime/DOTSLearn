using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
    public partial struct CameraControlSystem : ISystem
    {
        private const float CameraSpeed = 20f;

        public void OnCreate(ref SystemState state)
        {
            var em = state.EntityManager;
            
            // create camera archetype
            var cameraArchetype = em.CreateArchetype(
                typeof(CameraComponent),
                typeof(MovableComponent));
            
            // create camera entity with archetype
            var cameraEntity = em.CreateEntity(cameraArchetype);
            
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
                instance.transform.Translate(camera.Direction * dt * camera.Speed, Space.World);
            }
        }
    }
}