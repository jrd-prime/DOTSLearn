﻿using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.CommonData.Storage.Service;
using Sources.Scripts.Game.Features.Building.ControlPanel.System;
using Sources.Scripts.Timer;
using Sources.Scripts.UI;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage.WarehouseBox
{
    /// <summary>
    /// Moving products from main storage to building warehouse<br/>
    /// <see cref="MoveToWarehouseRequestTag"/>
    /// </summary>
    [BurstCompile]
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildingControlPanelSystem))]
    public partial class MoveToWarehouseRequestSystem : SystemBase
    {
        private BuildingDataAspect _aspect;
        private EntityCommandBuffer _ecbBI;
        private MainStorageBoxData _mainStorageBox;
        private NativeList<ProductData> _matchingProducts;

        protected override void OnCreate()
        {
            RequireForUpdate<MainStorageBoxData>();
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
            _ecbBI = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(World.Unmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<MoveToWarehouseRequestTag>())
            {
                Debug.LogWarning("___ REQUEST: Move To Warehouse");
                _ecbBI.RemoveComponent<MoveToWarehouseRequestTag>(aspect.Self);

                _aspect = aspect;
                _mainStorageBox = SystemAPI.GetSingleton<MainStorageBoxData>();

                _matchingProducts = StorageService.GetMatchingProducts(
                    _aspect.RequiredProductsData.Value,
                    _mainStorageBox.Value, out bool isEnough);


                if (!isEnough)
                {
                    TextPopUpMono.Instance.ShowPopUp("not enough products to move!".ToUpper());
                    return;
                }

                SetProductsToDelivery();
                SetDeliveryTimer();
                ReduceProductsInMainStorage();

                aspect.BuildingData.BuildingEvents.Enqueue(BuildingEvent.MoveToWarehouse_Timer_Started);
            }
        }

        private void SetProductsToDelivery()
        {
            _aspect.SetProductsToDelivery(_matchingProducts);

            //TODO refact/remove
            _ecbBI.AddComponent(_aspect.Self, new ProductsToDeliveryData { Value = _matchingProducts });
        }


        private void SetDeliveryTimer()
        {
            int productsQuantity = StorageService.GetProductsQuantity(_matchingProducts);
            int moveDuration = productsQuantity / 5;

            new global::Sources.Scripts.Timer.JTimer().StartNewTimer(_aspect.Self, TimerType.MoveToWarehouse,
                moveDuration, _ecbBI);
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