using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    public partial struct TempPrefabSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (eventComponent, transform, entity) in SystemAPI
                         .Query<RefRO<MovingEventComponent>, RefRW<LocalTransform>>()
                         .WithAll<TempBuildingTag, MoveByPlayerTag>().WithEntityAccess())
            {
                var camdeg = SystemAPI.GetSingleton<CameraComponent>();

                if (SystemAPI.TryGetSingletonEntity<CameraComponent>(out var e))
                {
                    ecb.SetComponent(e, new FollowComponent { Target = entity });
                    ecb.RemoveComponent<MoveByPlayerTag>(e);
                    ecb.RemoveComponent<MovingEventComponent>(e);
                }


                float3 cameraDirection =
                    Quaternion.AngleAxis(camdeg.RotationAngleY, new float3(0, 1, 0)) *
                    eventComponent.ValueRO.Direction;


                var curr = transform.ValueRO.Position;
                Debug.Log("curr" + curr);
                transform.ValueRW.Position = curr + (cameraDirection * SystemAPI.Time.DeltaTime * -10);
                
                Debug.Log("transform.ValueRW.Position" + transform.ValueRW.Position);
                
                Debug.Log("cameraDirection * SystemAPI.Time.DeltaTime * -10" + cameraDirection * SystemAPI.Time.DeltaTime * -10);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}