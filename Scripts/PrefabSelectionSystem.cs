using Jrd.GameStates.MainGameState;
using Jrd.UserInput;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Jrd
{
    [BurstCompile]
    public partial struct PrefabSelectionSystem : ISystem // TODO +job
    {
        private const float RayDistance = 200f;
        private Entity _tempTargetEntity;
        private int _tempFingerId;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameStateData>();
            state.RequireForUpdate<PhysicsWorldSingleton>();
            state.RequireForUpdate<InputCursorData>();
            _tempFingerId = -1;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // not building state
            if (SystemAPI.GetSingleton<GameStateData>().CurrentGameState != GameState.BuildingState)
                return; //TODO refact

            // Click.
            if (Input.touchCount == 1) //TODO more than 1 touch???
            {
                Touch touch = Input.GetTouch(0);

                // Set temp finger id.
                if (_tempFingerId == -1) _tempFingerId = touch.fingerId;

                // Touch began.
                if (touch.fingerId == _tempFingerId && (touch.phase is not (TouchPhase.Ended or TouchPhase.Canceled)))
                {
                    // Temp target doesn't exist. Raycast..
                    if (_tempTargetEntity == Entity.Null)
                    {
                        InputCursorData inputCursor = SystemAPI.GetSingleton<InputCursorData>();

                        Ray ray = inputCursor.ClickToRay;

                        // If missed. Return..
                        if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity targetEntity)) return;

                        // Hit. Set temp target entity.
                        _tempTargetEntity = targetEntity;
                    }

                    Debug.Log("Do stuff.");
                }
                else
                {
                    // Touch ended or cancelled or TouchID doesn't match.

                    // Reset temp finger id.
                    _tempFingerId = -1;

                    // Reset temp target entity.
                    _tempTargetEntity = Entity.Null;
                }
            }
        }

        [BurstCompile]
        public bool Raycast(float3 from, float3 to, out Entity entity)
        {
            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

            var input = new RaycastInput
            {
                Start = from,
                End = to,
                Filter = new CollisionFilter
                {
                    BelongsTo = (uint)1 << 31,
                    CollidesWith = (uint)1 << 31,
                    GroupIndex = 0
                }
            };

            if (collisionWorld.CastRay(input, out RaycastHit hit))
            {
                entity = hit.Entity;
                return true;
            }

            entity = Entity.Null;
            return false;
        }
    }

    // [MaterialProperty("_jrd")]
    // public struct MyOwnColor : IComponentData
    // {
    //     public float4 Value;
    // }
}