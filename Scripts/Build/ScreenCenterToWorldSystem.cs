using Unity.Entities;
using Unity.Mathematics;

namespace Jrd.Build
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    public partial struct ScreenCenterToWorldSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var coords = new float3();
            foreach (var q in SystemAPI.Query<RefRW<ScreenCenterToWorldComponent>>())
            {
                q.ValueRW.ScreenCenterToWorld = coords;
            }
        }
    }
}