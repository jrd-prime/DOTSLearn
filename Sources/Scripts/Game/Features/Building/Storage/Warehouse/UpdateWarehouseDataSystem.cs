using System;
using Sources.Scripts.Game.Features.Building.Storage.Service;
using Sources.Scripts.JTimer;
using Sources.Scripts.JTimer.Component;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage.Warehouse
{
    public partial struct UpdateWarehouseDataSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToDeliveryData>();
            state.RequireForUpdate<TimerData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var time = new TimeWrapper(DateTime.Now);
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var (aspect, productsToDeliveryData, timer) in SystemAPI
                         .Query<BuildingDataAspect, ProductsToDeliveryData, TimerData>())
            {
                if (!(timer.FinishTime.Value.Ticks <= time.Value.Ticks)) continue;

                StorageService.ChangeProductsQuantity(
                    aspect.BuildingProductsData.WarehouseData.Value,
                    Operation.Increase,
                    productsToDeliveryData.Value);

                // ecb.AddComponent<WarehouseDataUpdatedEvent>(aspect.Self);

                ecb.RemoveComponent<ProductsToDeliveryData>(aspect.Self);
            }
        }
    }
}