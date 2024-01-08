using Jrd.Utils.Const;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

namespace Jrd
{
    public partial struct PrefabSelectionSystem : ISystem
    {
        private Camera _camera;
        private PhysicsWorldSingleton _buildPhysicsWorld;
        private CollisionWorld _collisionWorld;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
            _camera = Camera.main;
            _buildPhysicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _collisionWorld = _buildPhysicsWorld.PhysicsWorld.CollisionWorld;
                var ray = _camera.ScreenPointToRay(Input.mousePosition);

                var rayStart = ray.origin;
                var rayEnd = ray.GetPoint(100f);

                var raycastInput = new RaycastInput
                {
                    Start = rayStart,
                    End = rayEnd,
                    Filter = new CollisionFilter
                    {
                        BelongsTo = unchecked((uint)CollisionLayers.Selectable),
                    }
                };
            }
        }
    }
}