using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    public partial struct MoveTempPrefabSystem : ISystem
    {
        private const float RayDistance = 200f;
        private const uint GroundLayer = 1u << 3;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<CameraComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);

            var touch = Input.GetTouch(0);

            Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(touch.position);

            if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out float3 hitPosition)) return;


            foreach (var (move, transform, tempPrefabEntity) in SystemAPI
                         .Query<RefRO<MoveDirectionData>, RefRW<LocalTransform>>()
                         .WithAll<TempBuildingTag, SelectedTag, SelectableTag>().WithEntityAccess())
            {
                var pos = new float3(math.round(hitPosition.x), 0, math.round(hitPosition.z));
                
                // Debug.Log(pos);
                transform.ValueRW.Position = pos;
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }


        public bool Raycast(float3 from, float3 to, out float3 hitPosition)
        {
            var input = new RaycastInput
            {
                Start = from,
                End = to,
                Filter = new CollisionFilter
                {
                    BelongsTo = GroundLayer,
                    CollidesWith = GroundLayer,
                    GroupIndex = 0
                }
            };

            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            if (collisionWorld.CastRay(input, out RaycastHit hit))
            {
                hitPosition = hit.Position;
                return true;
            }

            hitPosition = float3.zero;
            return false;
        }
    }
}