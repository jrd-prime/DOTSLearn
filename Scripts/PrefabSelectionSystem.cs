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
    public partial class PrefabSelectionSystem : SystemBase
    {
        private CollisionWorld _collisionWorld;
        private const float RayDistance = 200f;

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
        }

        protected override void OnUpdate()
        {
            if (Input.touchCount == 1)
            {
                Debug.Log("Click");
                if (!SystemAPI.TryGetSingleton(out InputCursorData inputCursor)) return;

                Ray ray = CameraMono.Instance.Camera.ScreenPointToRay(inputCursor.CursorScreenPosition);
                float3 rayFrom = ray.origin;
                float3 rayTo = ray.GetPoint(RayDistance);

                if (Raycast(rayFrom, rayTo, out Entity entity))
                {
                    Debug.Log("Entity = " + entity);

                    if (inputCursor.CursorState == CursorState.ClickAndHold)
                    {
                        // camera stop, prefab move
                        Debug.Log("move");
                    }
                }
            }
        }

        public bool Raycast(float3 from, float3 to, out Entity entity)
        {
            _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;

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

            if (_collisionWorld.CastRay(input, out RaycastHit hit))
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