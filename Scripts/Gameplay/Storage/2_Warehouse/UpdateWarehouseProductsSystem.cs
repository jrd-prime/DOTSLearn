using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage._2_Warehouse
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

            foreach (var (aspect, productsToDeliveryData, timer) in SystemAPI
                         .Query<BuildingDataAspect, ProductsToDeliveryData, ProductsMoveTimerData>())
            {
                if (!(timer.CurrentValue <= 0)) continue;

                Debug.Log("UPDATE PRODS");

                StorageService.ChangeProductsQuantity(
                    aspect.BuildingProductsData.WarehouseProductsData,
                    Operation.Increase,
                    productsToDeliveryData.Value);

                ecb.RemoveComponent<ProductsToDeliveryData>(aspect.Self);
            }
        }
    }
}