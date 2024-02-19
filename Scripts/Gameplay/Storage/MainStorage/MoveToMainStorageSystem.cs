using Jrd.Gameplay.Building;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.Service;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    public partial struct MoveToMainStorageSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<MoveToMainStorageRequestTag>())
            {
                NativeList<ProductData> productsList =
                    StorageService.GetProductsDataList(aspect.BuildingProductsData.ManufacturedBoxData.Value);

                aspect.ChangeProductsQuantity(new ChangeProductsQuantityData
                {
                    ChangeType = ChangeType.Reduce,
                    StorageType = StorageType.Manufactured,
                    ProductsData = productsList
                });

                aspect.ChangeProductsQuantity(new ChangeProductsQuantityData
                {
                    ChangeType = ChangeType.Increase,
                    StorageType = StorageType.Main,
                    ProductsData = productsList
                });

                ecb.AddComponent(aspect.Self, new AddEventToBuildingData
                {
                    Value = BuildingEvent.ManufacturedBoxDataUpdated
                });
            }
        }
    }
}