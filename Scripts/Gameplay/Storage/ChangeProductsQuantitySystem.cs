﻿using System;
using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage._1_MainStorage.Component;
using Jrd.Gameplay.Storage._3_InProduction.Component;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage._3_InProduction
{
    public partial struct ChangeProductsQuantitySystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MainStorageData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (var (aspect, changeProductsQuantityData)in SystemAPI
                         .Query<BuildingDataAspect, RefRO<ChangeProductsQuantityData>>())
            {
                Debug.Log("--- Change Quantity");
                StorageType storageType = changeProductsQuantityData.ValueRO.StorageType;
                ChangeType changeType = changeProductsQuantityData.ValueRO.ChangeType;
                NativeList<ProductData> productsData = changeProductsQuantityData.ValueRO.ProductsData;

                var warehouse = aspect.BuildingProductsData.WarehouseData;
                var inProduction = aspect.BuildingProductsData.InProductionData;
                var manufactured = aspect.BuildingProductsData.ManufacturedData;

                switch (storageType)
                {
                    case StorageType.Main:
                        MainStorageData mainStorage = SystemAPI.GetSingleton<MainStorageData>();
                        mainStorage.ChangeProductsQuantity(changeType, productsData);
                        break;

                    case StorageType.Warehouse:
                        warehouse.ChangeProductsQuantity(changeType, productsData);
                        break;

                    case StorageType.InProduction:
                        inProduction.ChangeProductsQuantity(changeType, productsData);
                        break;

                    case StorageType.Manufactured:
                        manufactured.ChangeProductsQuantity(changeType, productsData);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}