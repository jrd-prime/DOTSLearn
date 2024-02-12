using Jrd.Gameplay.Building;
using Jrd.Gameplay.Storage.MainStorage;
using Jrd.Gameplay.Timers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Products
{
    /// <summary>
    /// Moving products from main storage to building warehouse
    /// </summary>
    [BurstCompile]
    public partial struct MoveToWarehouseSystem : ISystem
    {
        private BuildingDataAspect _aspect;
        private EntityCommandBuffer _ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToWarehouseRequestTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<ProductsToWarehouseRequestTag>())
            {
                _aspect = aspect;

                MainStorageData mainStorage = SystemAPI.GetSingleton<MainStorageData>();

                NativeList<ProductData> matchingProducts =
                    mainStorage.GetMatchingProducts(aspect.RequiredProductsData.Required, Allocator.Persistent);

                SetProductsToDelivery(matchingProducts);
                SetDeliveryTimer(matchingProducts);
                ReduceProductsInMainStorage();

                _ecb.RemoveComponent<ProductsToWarehouseRequestTag>(aspect.Self);
            }
        }

        private void SetProductsToDelivery(NativeList<ProductData> productsList)
        {
            _ecb.AddComponent(_aspect.Self, new ProductsToDeliveryData { Value = productsList });
        }

        private void SetDeliveryTimer(NativeList<ProductData> productsList)
        {
            var movedProductsQuantity = GetProductsQuantity(productsList) / 4;

            _ecb.AddComponent(_aspect.Self, new ProductsMoveTimerData
            {
                StarValue = movedProductsQuantity,
                CurrentValue = movedProductsQuantity
            });

            Debug.Log("Timer set. Move time: " + movedProductsQuantity + " sec.");
        }

        private void ReduceProductsInMainStorage()
        {
            _ecb.AddComponent<ReduceMainStorageProductsTag>(_aspect.Self);
        }

        [BurstCompile]
        private int GetProductsQuantity(NativeList<ProductData> productData)
        {
            int quantity = 0;

            foreach (var product in productData)
            {
                quantity += product.Quantity;
            }

            return quantity;
        }
    }
}