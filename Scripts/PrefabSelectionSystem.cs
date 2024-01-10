using Jrd.JCamera;
using Jrd.UserInput;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Jrd
{
    public partial class PrefabSelectionSystem : SystemBase // TODO unmanaged rework, +job + burst
    {
        private const float RayDistance = 200f;
        private Entity _tempTargetEntity;
        private int _tempFingerId;

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
            RequireForUpdate<InputCursorData>();
            _tempFingerId = -1;
        }

        protected override void OnUpdate()
        {
            // not building state
            //  if (SystemAPI.GetSingleton<GameStateData>().CurrentGameState != GameState.BuildingState) return; //TODO refact

            // Click.
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                // Set FingerID.
                if (_tempFingerId == -1) _tempFingerId = touch.fingerId;

                // Touch began.
                if (touch.fingerId == _tempFingerId && (touch.phase is not (TouchPhase.Ended or TouchPhase.Canceled)))
                {
                    Debug.Log("Touch began.");

                    // Temp target entity doesn't exists
                    if (_tempTargetEntity == Entity.Null)
                    {
                        Debug.Log("Target doesn't exist. Raycast..");
                        InputCursorData inputCursor = SystemAPI.GetSingleton<InputCursorData>();

                        // Raycast.
                        Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(inputCursor.CursorScreenPosition);

                        if (!Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity targetEntity))
                        {
                            Debug.Log("Missed. Return..");
                            return;
                        }

                        Debug.Log("Hit. Set target");

                        // Set temp target entity.
                        _tempTargetEntity = targetEntity;
                    }
                    
                    Debug.Log("Do stuff.");
                    
                }
                else
                {
                    // Touch ended or cancelled or TouchID doesn't match.
                    Debug.Log("Touch ended or cancelled or TouchID doesn't match.");

                    // Reset temp finger id.
                    _tempFingerId = -1;

                    // Reset temp target entity.
                    _tempTargetEntity = Entity.Null;
                }
            }
        }

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