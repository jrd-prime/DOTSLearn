using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
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
            DynamicBuffer<BuildingManufacturedItemsBuffer> manufacturedItems =
                SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

            // To building
            foreach (var (buildingData, warehouseProductsData, entity) in SystemAPI
                         .Query<BuildingData, RefRW<WarehouseProductsData>>().WithAll<MoveRequestTag>()
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

                Debug.Log("Timer set");
                _ecb.AddComponent(entity, new ProductsMoveTimerData
                {
                    StarValue = 10,
                    CurrentValue = 10
                });
            }
        }

        private void MoveMatchingProducts(MainStorageData mainStorageData, WarehouseProductsData warehouseProductsData,
            DynamicBuffer<BuildingRequiredItemsBuffer> buildingRequiredItemsBuffers)
        {
        }
    }
}