using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.Gameplay.Products
{
    public partial struct ProductsMoverSystem : ISystem
    {
        private EntityCommandBuffer _ecb;
        private MainStorageData _mainStorageData;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<JBuildingsPrefabsTag>();
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MoveRequestTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
            // cache mb somewhere
            DynamicBuffer<BuildingRequiredItemsBuffer> requiredItems =
                SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);

            // To building
            foreach (var (warehouseProductsData, entity) in SystemAPI
                         .Query<RefRW<WarehouseProductsData>>()
                         .WithAll<MoveRequestTag, BuildingData>()
                         .WithEntityAccess())
            {
                _ecb.RemoveComponent<MoveRequestTag>(entity);

                WarehouseProductsData productsData = warehouseProductsData.ValueRW;

                // matching
                NativeList<ProductData> matchingProductsList = _mainStorageData.GetMatchingProducts(requiredItems);
                // move and return moved count
                NativeList<ProductData> movedProductsList = productsData.UpdateProductsCount(matchingProductsList);
                // update quantity moved products in main storage
                _mainStorageData.UpdateProductsByKey(movedProductsList);

                var count = 0;

                foreach (var q in movedProductsList)
                {
                    //TODO get multiplier
                    // count += (int)math.round(q.Quantity * q.MoveTimeMultiplier);
                    count += q.Quantity;
                }

                Debug.Log("Timer set. Move time: " + count);
                _ecb.AddComponent(entity, new ProductsMoveTimerData
                {
                    StarValue = count,
                    CurrentValue = count,
                });
            }
        }

        private void MoveMatchingProducts(MainStorageData mainStorageData, WarehouseProductsData warehouseProductsData,
            DynamicBuffer<BuildingRequiredItemsBuffer> buildingRequiredItemsBuffers)
        {
        }
    }
}