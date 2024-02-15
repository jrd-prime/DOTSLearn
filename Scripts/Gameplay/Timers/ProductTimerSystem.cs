using Jrd.Gameplay.Building;
using Jrd.Gameplay.Storage._1_MainStorage.Component;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Timers
{
    public partial struct ProductTimerSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private MainStorageData _mainStorageData;
        private float tempTime;
        private bool tick;
        private float tickDelay;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);
            // Debug.LogWarning(Time.fixedTime);

            tick = false;
            tickDelay = 0.5f;
            if (Time.fixedTime > tempTime)
            {
                tempTime = Time.fixedTime + tickDelay;
                Debug.LogWarning("temp time = " + tempTime);
            }

            if ((tempTime - Time.fixedTime) < 0.001)
            {
                Debug.LogWarning(Time.fixedTime + " / tick?");
                tempTime = Time.fixedTime + tickDelay;
                tick = true;
            }


            foreach (var (aspect, timer) in SystemAPI
                         .Query<BuildingDataAspect, RefRW<ProductsMoveTimerData>>())
            {
                switch (timer.ValueRO.CurrentValue)
                {
                    case > 0:
                        if (!tick) return;
                        timer.ValueRW.CurrentValue -= tickDelay;
                        break;
                    case <= 0:
                        _ecb.RemoveComponent<ProductsMoveTimerData>(aspect.Self);
                        break;
                }
            }

            foreach (var (aspect, all) in SystemAPI
                         .Query<BuildingDataAspect, RefRW<AllLoadedProductsTimerData>>())
            {
                switch (all.ValueRO.Value)
                {
                    case > 0:
                        if (!tick) return;
                        Debug.LogWarning("all timer > 0");
                        all.ValueRW.Value -= tickDelay;
                        break;
                    case <= 0:
                        Debug.LogWarning("all timer <= 0");
                        _ecb.AddComponent<AllLoadedProductsTimerFinishedEvent>(aspect.Self);
                        _ecb.RemoveComponent<AllLoadedProductsTimerData>(aspect.Self);
                        break;
                }
            }

            foreach (var (aspect, one) in SystemAPI
                         .Query<BuildingDataAspect, RefRW<OneLoadedProductTimerData>>())
            {
                switch (one.ValueRO.Value)
                {
                    case > 0:
                        Debug.LogWarning("one timer > 0");
                        if (!tick) return;
                        one.ValueRW.Value -= tickDelay;
                        break;
                    case <= 0:
                        Debug.LogWarning("one timer <= 0");
                        _ecb.AddComponent<OneLoadedProductsTimerFinishedEvent>(aspect.Self);
                        _ecb.RemoveComponent<OneLoadedProductTimerData>(aspect.Self);
                        break;
                }
            }
        }
    }
}