﻿using System;
using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Storage;
using Sources.Scripts.CommonComponents.Storage.Data;
using Sources.Scripts.Game.Features.Building.ControlPanel.System;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    [UpdateBefore(typeof(BuildingControlPanelSystem))]
    public partial struct ChangeProductsQuantitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MainStorageBoxData>();
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
                            MainStorageBoxData mainStorageBox = SystemAPI.GetSingleton<MainStorageBoxData>();
                            mainStorageBox.ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.MainStorageDataUpdated);
                            break;

                        case StorageType.Warehouse:
                            aspect.ProductsInBuildingData.WarehouseBoxData
                                .ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.WarehouseDataUpdated);
                            break;

                        case StorageType.InProduction:
                            aspect.ProductsInBuildingData.InProductionBoxData
                                .ChangeProductsQuantity(changeType, productsData);

                            buildingEvents.Enqueue(BuildingEvent.InProductionBoxDataUpdated);
                            break;

                        case StorageType.Manufactured:
                            aspect.ProductsInBuildingData.ManufacturedBoxData
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