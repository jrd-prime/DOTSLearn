﻿using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Timers;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.Warehouse
{
    public partial struct UpdateWarehouseProductsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToDeliveryData>();
            state.RequireForUpdate<ProductsMoveTimerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (aspect, products, timer) in SystemAPI
                         .Query<BuildingDataAspect, ProductsToDeliveryData, ProductsMoveTimerData>())
            {
                if (!(timer.CurrentValue <= 0)) continue;
                Debug.Log("UPDATE PRODS");
                aspect.BuildingProductsData.WarehouseProductsData.IncreaseProductsQuantity(products.Value);
                ecb.RemoveComponent<ProductsToDeliveryData>(aspect.Self);
            }
        }
    }
}