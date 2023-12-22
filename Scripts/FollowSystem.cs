using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd
{
    public partial struct FollowSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (followComponent, cameraComponent, entity) in SystemAPI
                         .Query<RefRO<FollowComponent>, RefRO<CameraComponent>>()
                         .WithEntityAccess())
            {
                if (followComponent.ValueRO.Target == Entity.Null) return;

                var target = followComponent.ValueRO.Target;
                var targetPosition = SystemAPI.GetComponentRW<LocalTransform>(target).ValueRO.Position;

                // SystemAPI.GetComponentRW<LocalTransform>(entity).ValueRW.Position = targetPosition;

                var instance = CameraSingleton.Instance; // camera ref

                if (instance == null) return;

                var position = new float3(0,10,-10);

                instance.Camera.transform.position = position;
                

                var del = targetPosition - position;


                Debug.LogWarning("position " + position);
                Debug.LogWarning("Camera.transform.position " + instance.Camera.transform.position);
                Debug.LogWarning("targetPosition " + targetPosition);
                Debug.LogWarning("del " + del);


                position = targetPosition;
                // instance.Camera.transform.position = position;
            }
        }
    }
}