using Sources.Scripts.CommonComponents;
using Sources.Scripts.Timer;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
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
                         .Query<CommonComponents.test.BuildingDataAspect, RefRW<TimerData>>()
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