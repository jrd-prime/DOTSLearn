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
        private Camera _camera;
        private PhysicsWorldSingleton _buildPhysicsWorld;
        private CollisionWorld _collisionWorld;

        protected override void OnCreate()
        {
            RequireForUpdate<PhysicsWorldSingleton>();
        }

        protected override void OnUpdate()
        {
            var inputCursorData = SystemAPI.GetSingleton<InputCursorData>();
            
            
            _camera = CameraMono.Instance.Camera;
            
            if (Input.touchCount == 1)
            {
                Ray ray = _camera.ScreenPointToRay(inputCursorData.CursorScreenPosition);

                Vector3 rayStart = ray.origin;
                Vector3 rayEnd = ray.GetPoint(200f);

                if (Raycast(rayStart, rayEnd, out Entity entity))
                {
                    Debug.Log(entity);
                }

                if (inputCursorData.CursorState == CursorState.ClickAndHold)
                {
                    Debug.Log("do move");
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
}