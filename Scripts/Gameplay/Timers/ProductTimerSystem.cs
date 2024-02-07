using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Timers
{
    public partial struct ProductTimerSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private MainStorageData _mainStorageData;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<JBuildingsPrefabsTag>();
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);


            foreach (var (buildingData, timer, entity) in SystemAPI
                         .Query<BuildingData, RefRW<ProductsMoveTimerData>>()
                         .WithEntityAccess())
            {
                if (timer.ValueRO.CurrentValue > 0)
                {
                    Debug.LogWarning("timer > 0");
                    timer.ValueRW.CurrentValue -= SystemAPI.Time.DeltaTime;
                    return;
                }

                if (timer.ValueRO.CurrentValue <= 0)
                {
                    Debug.LogWarning("timer <= 0");
                }
            }
        }
    }
}