using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage._1_MainStorage;
using Jrd.Gameplay.Storage._1_MainStorage.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage._2_Warehouse
{
    /// <summary>
    /// Moving products from main storage to building warehouse<br/>
    /// <see cref="ProductsToWarehouseRequestTag"/>
    /// </summary>
    [BurstCompile]
    public partial struct MoveToWarehouseSystem : ISystem
    {
        private BuildingDataAspect _aspect;
        private EntityCommandBuffer _ecb;
        private MainStorageData _mainStorage;
        private NativeList<ProductData> _matchingProducts;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToWarehouseRequestTag>();
        }

        public void OnDestroy(ref SystemState state)
        {
            _matchingProducts.Dispose();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            _ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<ProductsToWarehouseRequestTag>())
            {
                _aspect = aspect;

                _mainStorage = SystemAPI.GetSingleton<MainStorageData>();

                _matchingProducts = StorageService.GetMatchingProducts(
                    aspect.RequiredProductsData.Required,
                    _mainStorage.Values,
                    Allocator.Persistent);

                SetProductsToDelivery();
                SetDeliveryTimer();
                ReduceProductsInMainStorage();

                _ecb.AddComponent<MainStorageDataUpdatedEvent>(aspect.Self);

                _ecb.RemoveComponent<ProductsToWarehouseRequestTag>(aspect.Self);
            }
        }

        private void SetProductsToDelivery() =>
            _ecb.AddComponent(_aspect.Self, new ProductsToDeliveryData { Value = _matchingProducts });


        private void SetDeliveryTimer()
        {
            int productsQuantity = StorageService.GetProductsQuantity(_matchingProducts);

            int movedProductsQuantity = productsQuantity / 4;

            _ecb.AddComponent(_aspect.Self, new ProductsMoveTimerData
            {
                StarValue = movedProductsQuantity,
                CurrentValue = movedProductsQuantity
            });

            Debug.Log("Timer set. Move time: " + movedProductsQuantity + " sec.");
        }

        private void ReduceProductsInMainStorage() =>
            StorageService.ChangeProductsQuantity(_mainStorage.Values, Operation.Reduce, _matchingProducts);
    }
}