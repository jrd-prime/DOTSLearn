using GamePlay.Features.Building.Events;
using GamePlay.Features.Building.Products.Component;
using GamePlay.Features.Building.Storage.InProductionBox.Component;
using GamePlay.Features.Building.Storage.MainStorage.Component;
using GamePlay.Features.Building.Storage.Service;
using GamePlay.UI;
using JTimer;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Features.Building.Storage.Warehouse
{
    /// <summary>
    /// Moving products from main storage to building warehouse<br/>
    /// <see cref="MoveToWarehouseRequestTag"/>
    /// </summary>
    [BurstCompile]
    public partial class MoveToWarehouseRequestSystem : SystemBase
    {
        private BuildingDataAspect _aspect;
        private EntityCommandBuffer _ecb;
        private MainStorageData _mainStorage;
        private NativeList<ProductData> _matchingProducts;

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageData>();
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            RequireForUpdate<MoveToWarehouseRequestTag>();
        }

        protected override void OnDestroy()
        {
            _matchingProducts.Dispose();
        }

        [BurstCompile]
        protected override void OnUpdate()
        {
            _ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

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

                if (!isEnough)
                {
                    TextPopUpMono.Instance.ShowPopUp("not enough products to move!".ToUpper());
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

            new JTimer.JTimer().StartNewTimer(_aspect.Self, TimerType.MoveToWarehouse, moveDuration, _ecb);
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