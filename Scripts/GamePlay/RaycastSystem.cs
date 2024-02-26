using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace GamePlay
{
    public partial struct RaycastSystem : ISystem
    {
        private const float RayDistance = 200f;
        private static CollisionWorld _collisionWorld;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        }

        private static bool RaycastCast(Ray ray, uint layer, out RaycastHit hit)
        {
            Vector3 from = ray.origin;
            Vector3 to = ray.GetPoint(RayDistance);

            var input = new RaycastInput
            {
                Start = from,
                End = to,
                Filter = new CollisionFilter
                {
                    BelongsTo = layer,
                    CollidesWith = layer,
                    GroupIndex = 0
                }
            };

            hit = new RaycastHit();

            if (!_collisionWorld.CastRay(input, out RaycastHit raycastHit)) return false;

            hit = raycastHit;
            return true;
        }

        public static bool Raycast(Ray ray, uint layer, out RaycastHit hit, float distance = 100f)
        {
            hit = new RaycastHit();

            if (!RaycastCast(ray, layer, out RaycastHit raycastHit)) return false;

            hit = raycastHit;
            return true;
        }
    }
}