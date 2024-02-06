using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Product
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
            state.RequireForUpdate<MoveRequestComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            _mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            // To building
            foreach (var (moveRequestComponent, buildingData, entity) in SystemAPI
                         .Query<RefRO<MoveRequestComponent>, RefRW<BuildingData>>()
                         .WithEntityAccess())
            {
                _ecb.RemoveComponent<MoveRequestComponent>(entity);

                var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
                // cache mb somewhere
                DynamicBuffer<BuildingRequiredItemsBuffer> requiredItems =
                    SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
                DynamicBuffer<BuildingManufacturedItemsBuffer> manufacturedItems =
                    SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

                var warehouseProductsData =
                    SystemAPI.GetComponentRW<WarehouseProductsData>(moveRequestComponent.ValueRO.Value);

                foreach (var q in warehouseProductsData.ValueRO.Values)
                {
                    Debug.LogWarning(q.Key);
                }

                MoveMatchingProducts(_mainStorageData, warehouseProductsData.ValueRW, requiredItems);

                Debug.LogWarning(
                    "move request from: " + state.EntityManager.GetName(moveRequestComponent.ValueRO.Value));
            }
        }

        private void MoveMatchingProducts(MainStorageData mainStorageData, WarehouseProductsData warehouseProductsData,
            DynamicBuffer<BuildingRequiredItemsBuffer> buildingRequiredItemsBuffers)
        {
            var a = mainStorageData.GetMatchingProducts(buildingRequiredItemsBuffers);
            var b = warehouseProductsData.UpdateProductsCount(a);
            
        }
    }
}