using Unity.Entities;
using Unity.Physics;
using Ray = UnityEngine.Ray;

namespace Jrd
{
    public partial struct RaycastSystem : ISystem
    {
        private static CollisionWorld _collisionWorld;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PhysicsWorldSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
        }

        public static bool Raycast(Ray ray, uint targetLayer, out Entity entity, float distance = 100f)
        {
            // from: ray.origin,
            // to: ray.GetPoint(RayDistance),
            // targetLayer: TargetLayer,

            var from = ray.origin;
            var to = ray.GetPoint(distance);


            var input = new RaycastInput
            {
                Start = from,
                End = to,
                Filter = new CollisionFilter
                {
                    BelongsTo = targetLayer,
                    CollidesWith = targetLayer,
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