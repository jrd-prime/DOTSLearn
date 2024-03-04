using Sources.Scripts.CommonComponents;
using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.CommonComponents.Storage;
using Sources.Scripts.CommonComponents.Storage.Data;
using Sources.Scripts.CommonComponents.Storage.Service;
using Sources.Scripts.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage.InProductionBox
{
    /// <summary>
    /// Move products from the building warehouse to production box<br/>
    /// <see cref="MoveToProductionBoxRequestTag"/>
    /// </summary>
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial struct MoveToProductionBoxSystem : ISystem
    {
        private BuildingDataAspect _aspect;
        private WarehouseBoxData _warehouseBoxData;
        private NativeList<ProductData> _requiredQuantity;
        private NativeList<ProductData> _preparedProducts;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<MoveToProductionBoxRequestTag>();
        }

        public void OnDestroy(ref SystemState state)
        {
            _preparedProducts.Dispose();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<MoveToProductionBoxRequestTag>())
            {
                ecb.RemoveComponent<MoveToProductionBoxRequestTag>(aspect.BuildingData.Self);

                Debug.LogWarning("___ REQUEST: Move To Production Box");

                _aspect = aspect;
                _warehouseBoxData = aspect.ProductsInBuildingData.WarehouseBoxData;
                _requiredQuantity = aspect.RequiredProductsData.Value;

                bool isEnoughProductsToLoad =
                    WarehouseService.IsEnoughRequiredProducts(_warehouseBoxData, _requiredQuantity);

                if (isEnoughProductsToLoad)
                {
                    _preparedProducts = WarehouseService.GetProductsForProductionAndMaxLoads(
                        _warehouseBoxData,
                        _requiredQuantity,
                        aspect.BuildingData.LoadCapacity, out int maxLoads);

                    if (_preparedProducts.IsEmpty) Debug.LogError("PrepProd list is empty!");

                    ChangeProductsQuantity(_preparedProducts);
                    AddEventsForUpdateUI();
                    SetProductionProcessData(maxLoads);

                    aspect.SetProductionState(ProductionState.EnoughProducts);

                    // 5 production settings to building SO
                }
                else
                {
                    TextPopUpMono.Instance.ShowPopUp("Insufficient products to produce!".ToUpper());
                }
            }
        }

        private void SetProductionProcessData(int maxLoads)
        {
            _aspect.SetPreparedProductsToProduction(_preparedProducts);
            _aspect.SetMaxLoads(maxLoads);
        }

        private void AddEventsForUpdateUI()
        {
            _aspect.BuildingData.BuildingEvents.Enqueue(BuildingEvent.MoveToProductionBoxFinished);
            _aspect.AddEvent(BuildingEvent.InProductionBoxDataUpdated);
        }

        private void ChangeProductsQuantity(NativeList<ProductData> preparedProducts)
        {
            _aspect.ChangeProductsQuantityData.Value.Enqueue(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Warehouse,
                ChangeType = ChangeType.Reduce,
                ProductsData = preparedProducts
            });

            _aspect.ChangeProductsQuantityData.Value.Enqueue(new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Increase,
                ProductsData = preparedProducts
            });
        }
    }
}