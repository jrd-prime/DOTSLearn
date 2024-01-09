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

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
        }

        protected override void OnUpdate()
        {
            // click
            if (Input.touchCount == 1)
            {
                Debug.Log("Click");

                if (!SystemAPI.TryGetSingleton(out InputCursorData inputCursor)) return;

                Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(inputCursor.CursorScreenPosition);

                // hit
                if (Raycast(ray.origin, ray.GetPoint(RayDistance), out Entity entity))
                {
                    Debug.Log("Hit. Entity = " + entity);

                    // still holding btn/finger
                    if (inputCursor.CursorState == CursorState.ClickAndHold)
                    {
                        // camera stop, prefab move
                        Debug.Log("camera stop, prefab move");
                    }
                    else
                    {
                        // prefab stop, camera move
                        Debug.Log("prefab stop, camera move");
                    }
                }
                else
                {
                    Debug.Log("Didn't hit.");
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