﻿using System;
using GamePlay.Building.ControlPanel;
using GamePlay.Building.SetUp;
using GamePlay.Products.Component;
using GamePlay.Storage.InProductionBox.Component;
using GamePlay.Storage.MainStorage.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Storage
{
    [UpdateBefore(typeof(BuildingControlPanelSystem))]
    public partial struct ChangeProductsQuantitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MainStorageData>();
            state.RequireForUpdate<ChangeProductsQuantityQueueData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (aspect, queueData)in SystemAPI
                         .Query<BuildingDataAspect, RefRW<ChangeProductsQuantityQueueData>>())
            {
                while (queueData.ValueRW.Value.Count > 0)
                {
                    var buildingEvents = aspect.BuildingData.BuildingEvents;
                    var dequeue = queueData.ValueRW.Value.Dequeue();

                    // Debug.LogWarning("___ CHANGE QUANTITY: " + dequeue.StorageType + "/" + dequeue.ChangeType);

                    ChangeType changeType = dequeue.ChangeType;
                    NativeList<ProductData> productsData = dequeue.ProductsData;

                    switch (dequeue.StorageType)
                    {
                        case StorageType.Main:
                            MainStorageData mainStorage = SystemAPI.GetSingleton<MainStorageData>();
                            mainStorage.ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.MainStorageDataUpdated);
                            break;

                        case StorageType.Warehouse:
                            aspect.BuildingProductsData.WarehouseData
                                .ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.WarehouseDataUpdated);
                            break;

                        case StorageType.InProduction:
                            aspect.BuildingProductsData.InProductionBoxData
                                .ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.InProductionBoxDataUpdated);
                            break;

                        case StorageType.Manufactured:
                            aspect.BuildingProductsData.ManufacturedBoxData
                                .ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.ManufacturedBoxDataUpdated);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }
}