using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Timers;
using Unity.Entities;

namespace Jrd.Gameplay.Storage._2_Warehouse
{
    public partial struct UpdateWarehouseDataSystem : ISystem
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

                StorageService.ChangeProductsQuantity(
                    aspect.BuildingProductsData.WarehouseData.Value,
                    Operation.Increase,
                    productsToDeliveryData.Value);

                ecb.AddComponent<WarehouseDataUpdatedEvent>(aspect.Self);

                ecb.RemoveComponent<ProductsToDeliveryData>(aspect.Self);
            }
        }
    }
}