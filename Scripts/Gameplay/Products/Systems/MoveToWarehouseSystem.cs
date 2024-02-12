using System.Threading.Tasks;
using Jrd.Gameplay.Building;
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
        private BuildingDataAspect _aspect;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToWarehouseRequestTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            MainStorageData mainStorageData = SystemAPI.GetSingleton<MainStorageData>();

            foreach (var (aspect, requiredProductsData, entity) in SystemAPI
                         .Query<BuildingDataAspect, RequiredProductsData>()
                         .WithAll<ProductsToWarehouseRequestTag>()
                         .WithEntityAccess())
            {
                _aspect = aspect;
                // 1 match
                NativeList<ProductData> matchingProducts =
                    mainStorageData.GetMatchingProducts(requiredProductsData.Required);

                // 2 timer
                var movedProductsQuantity = GetMatchingProductsQuantity(matchingProducts) /2;
                Debug.LogWarning($"matching products quantity = {GetMatchingProductsQuantity(matchingProducts)}");

                ecb.RemoveComponent<ProductsToWarehouseRequestTag>(entity);
                ecb.AddComponent(entity, new ProductsMoveTimerData
                {
                    StarValue = movedProductsQuantity,
                    CurrentValue = movedProductsQuantity,
                });

                // 3 reduce main
                // update quantity moved products in main storage
                mainStorageData.ReduceProductsQuantityByKey(matchingProducts);

                // 4 timer end - increase warehouse
                IncreaseProductsQuantityInWarehose(matchingProducts, movedProductsQuantity);

                Debug.Log("Timer set. Move time: " + movedProductsQuantity);
            }
        }

        private async void IncreaseProductsQuantityInWarehose(NativeList<ProductData> matchingProducts,
            int delay)
        {
            Debug.LogWarning("IN UPDATE1");
            await Task.Delay(delay);

            Debug.LogWarning("UPDATe START");
            _aspect.BuildingProductsData.WarehouseProductsData.UpdateProductsQuantity(matchingProducts);
        }


        private int GetMatchingProductsQuantity(NativeList<ProductData> matchingProducts)
        {
            var a = 0;

            foreach (var q in matchingProducts)
            {
                a += q.Quantity;
            }

            return a;
        }
    }
}