using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.MainStorage.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Jrd.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.Warehouse
{
    /// <summary>
    /// Moving products from main storage to building warehouse<br/>
    /// <see cref="MoveToWarehouseRequestTag"/>
    /// </summary>
    [BurstCompile]
    public partial struct MoveToWarehouseRequestSystem : ISystem
    {
        private BuildingDataAspect _aspect;
        private EntityCommandBuffer _ecb;
        private MainStorageData _mainStorage;
        private NativeList<ProductData> _matchingProducts;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MoveToWarehouseRequestTag>();
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
                         .WithAll<MoveToWarehouseRequestTag>())
            {
                Debug.LogWarning("___ REQUEST: Move To Warehouse");
                _ecb.RemoveComponent<MoveToWarehouseRequestTag>(aspect.Self);

                _aspect = aspect;
                _mainStorage = SystemAPI.GetSingleton<MainStorageData>();

                _matchingProducts = StorageService.GetMatchingProducts(
                    _aspect.RequiredProductsData.Required,
                    _mainStorage.Value, out bool isEnough);
                Debug.LogWarning(isEnough);
                if (!isEnough)
                {
                    Debug.LogWarning("not enough products to move");
                    return;
                }

                SetProductsToDelivery();
                SetDeliveryTimer();
                ReduceProductsInMainStorage();

                aspect.BuildingData.BuildingEvents.Enqueue(BuildingEvent.MoveToWarehouseTimerStarted);
            }
        }

        private void SetProductsToDelivery() =>
            _ecb.AddComponent(_aspect.Self, new ProductsToDeliveryData { Value = _matchingProducts });

        private void SetDeliveryTimer()
        {
            int productsQuantity = StorageService.GetProductsQuantity(_matchingProducts);
            int moveDuration = productsQuantity / 5;

            new JTimer().StartNewTimer(_aspect.Self, TimerType.MoveToWarehouse, moveDuration, _ecb);
        }

        private void ReduceProductsInMainStorage() =>
            _aspect.ChangeProductsQuantityData.Value.Enqueue(
                new ChangeProductsQuantityData
                {
                    StorageType = StorageType.Main,
                    ChangeType = ChangeType.Reduce,
                    ProductsData = _matchingProducts
                });
    }
}