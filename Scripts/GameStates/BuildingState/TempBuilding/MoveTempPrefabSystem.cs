using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    public partial struct MoveTempPrefabSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            foreach (var (move, transform, tempPrefabEntity) in SystemAPI
                         .Query<RefRO<MovingEventComponent>, RefRW<LocalTransform>>()
                         .WithAll<TempBuildingTag>().WithEntityAccess())
            {
                // set camera to follow this entity
                if (SystemAPI.TryGetSingletonEntity<CameraComponent>(out var e))
                {
                    ecb.AddComponent(e, new FollowComponent { Target = tempPrefabEntity });
                    ecb.RemoveComponent<MovingEventComponent>(e);
                }

                var cameraComponent = SystemAPI.GetSingleton<CameraComponent>();
                float3 cameraDirection =
                    Quaternion.AngleAxis(cameraComponent.RotationAngleY, Vector3.up) *
                    move.ValueRO.Direction;
                
                var currentPosition = transform.ValueRO.Position;
                transform.ValueRW.Position = currentPosition + (cameraDirection * SystemAPI.Time.DeltaTime * -10);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}