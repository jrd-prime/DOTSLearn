using JTimer.Component;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Features.Building.Storage
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
                         .Query<BuildingDataAspect, RefRW<TimerData>>()
                         .WithAll<InstantBuffTag>()
                         .WithEntityAccess())
            {
                Debug.LogWarning("Instant");
                // timer.ValueRW.FinishTime = Time.time;/
                _ecb.RemoveComponent<InstantBuffTag>(entity);
            }
        }
    }
}