using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.MainStorage.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
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
                Debug.LogWarning("REQUEST: Move To Warehouse");
                _aspect = aspect;
                _mainStorage = SystemAPI.GetSingleton<MainStorageData>();

                _matchingProducts = StorageService.GetMatchingProducts(
                    aspect.RequiredProductsData.Required,
                    _mainStorage.Value,
                    Allocator.Persistent);

                SetProductsToDelivery();
                SetDeliveryTimer();
                ReduceProductsInMainStorage();

                _ecb.RemoveComponent<MoveToWarehouseRequestTag>(aspect.Self);
                aspect.BuildingData.BuildingEvents.Add(BuildingEvent.MoveToWarehouseTimerStarted);
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
            StorageService.ChangeProductsQuantity(_mainStorage.Value, Operation.Reduce, _matchingProducts);
    }
}