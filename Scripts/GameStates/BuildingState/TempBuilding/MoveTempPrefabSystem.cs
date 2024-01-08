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
                         .Query<RefRO<MoveDirectionData>, RefRW<LocalTransform>>()
                         .WithAll<TempBuildingTag>().WithEntityAccess())
            {
                var cameraComponent = SystemAPI.GetSingleton<CameraComponent>();
                float3 cameraDirection =
                    Quaternion.AngleAxis(cameraComponent.RotationAngleY, Vector3.up) *
                    move.ValueRO.Direction;


                var a = math.round(move.ValueRO.Direction);


                // Debug.Log(a);
                // TODO https://app.asana.com/0/1206217975075068/1206234441074577/f
                var currentPosition = transform.ValueRO.Position;
                transform.ValueRW.Position = Vector3.Lerp(currentPosition, currentPosition + a, 100);
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}