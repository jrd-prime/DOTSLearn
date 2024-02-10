using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Products
{
    /// <summary>
    /// Moving products from main storage to building warehouse
    /// </summary>
    public partial struct MoveToWarehouseSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MoveRequestTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            MainStorageData mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            foreach (var (aspect, requiredProductsData, entity) in SystemAPI
                         .Query<BuildingDataAspect, RequiredProductsData>()
                         .WithAll<MoveRequestTag, BuildingData>()
                         .WithEntityAccess())
            {
                
                NativeList<ProductData> matchingProducts = mainStorageData.GetMatchingProducts(requiredProductsData.Required);
                // move and return moved count
                NativeList<ProductData> movedProducts =
                    aspect.BuildingProductsData.WarehouseProductsData.UpdateProductsQuantity(matchingProducts);
                // update quantity moved products in main storage
                mainStorageData.ReduceProductsQuantityByKey(movedProducts);

                var movedProductsQuantity = mainStorageData.GetProductsQuantity(movedProducts);

                Debug.Log("Timer set. Move time: " + movedProductsQuantity);

                ecb.RemoveComponent<MoveRequestTag>(entity);
                ecb.AddComponent(entity, new ProductsMoveTimerData
                {
                    StarValue = movedProductsQuantity,
                    CurrentValue = movedProductsQuantity,
                });
            }
        }
    }
}