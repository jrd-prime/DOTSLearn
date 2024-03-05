using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.CommonData.Storage.Service;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage.MainStorageBox
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
                         .Query<BuildingDataAspect>()
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
                    Value = BuildingEvent.ManufacturedBox_Updated
                });
            }
        }
    }
}