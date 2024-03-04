﻿using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.test;
using Sources.Scripts.CommonComponents.test.Service;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Sources.Scripts.Timer;
using Sources.Scripts.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage.Warehouse.System
{
    /// <summary>
    /// Moving products from main storage to building warehouse<br/>
    /// <see cref="MoveToWarehouseRequestTag"/>
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class MoveToWarehouseRequestSystem : SystemBase
    {
        private CommonComponents.test.BuildingDataAspect _aspect;
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
                         .Query<CommonComponents.test.BuildingDataAspect>()
                         .WithAll<MoveToWarehouseRequestTag>())
            {
                Debug.LogWarning("___ REQUEST: Move To Warehouse");
                _ecb.RemoveComponent<MoveToWarehouseRequestTag>(aspect.Self);

                _aspect = aspect;
                _mainStorage = SystemAPI.GetSingleton<MainStorageData>();

                _matchingProducts = StorageService.GetMatchingProducts(
                    _aspect.RequiredProductsData.Value,
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

            new global::Sources.Scripts.Timer.JTimer().StartNewTimer(_aspect.Self, TimerType.MoveToWarehouse,
                moveDuration, _ecb);
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