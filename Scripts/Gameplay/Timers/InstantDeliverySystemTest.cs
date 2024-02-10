using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.ControlPanel;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Timers
{
    public partial struct InstantDeliverySystemTest : ISystem
    {
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (aspect, timer, entity) in SystemAPI
                         .Query<BuildingDataAspect, RefRW<ProductsMoveTimerData>>()
                         .WithAll<InstantBuffTag>()
                         .WithEntityAccess())
            {
                Debug.LogWarning("Instant");
                timer.ValueRW.CurrentValue = 0;
                _ecb.RemoveComponent<InstantBuffTag>(entity);
            }
        }
    }
}