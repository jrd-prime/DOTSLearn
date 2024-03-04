using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.test;
using Sources.Scripts.CommonComponents.test.Service;
using Sources.Scripts.Game.Features.Building.Events;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage.MainStorage.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
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
                         .Query<CommonComponents.test.BuildingDataAspect>()
                         .WithAll<MoveToMainStorageRequestTag>())
            {
                NativeList<ProductData> productsList =
                    StorageService.GetProductsDataList(aspect.ProductsInBuildingData.ManufacturedBoxData.Value);

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