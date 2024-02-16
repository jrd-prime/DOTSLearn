using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.Production;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.Gameplay.Storage.Warehouse.Component;
using Jrd.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.InProductionBox
{
    /// <summary>
    /// Move products from the building warehouse to production box<br/>
    /// <see cref="ProductsToProductionBoxRequestTag"/>
    /// </summary>
    public partial struct MoveToProductionBoxSystem : ISystem
    {
        private WarehouseData _warehouseData;
        private InProductionBoxData _productionBoxData;
        private NativeList<ProductData> _requiredQuantity;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<ProductsToProductionBoxRequestTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = SystemAPI
                .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                .CreateCommandBuffer(state.WorldUnmanaged);

            foreach (var aspect in SystemAPI
                         .Query<BuildingDataAspect>()
                         .WithAll<ProductsToProductionBoxRequestTag>())
            {
                Debug.LogWarning("to production box request");

                _warehouseData = aspect.BuildingProductsData.WarehouseData;
                _productionBoxData = aspect.BuildingProductsData.InProductionBoxData;
                _requiredQuantity = aspect.RequiredProductsData.Required;

                bool isEnoughProducts =
                    WarehouseService.IsEnoughRequiredProducts(_warehouseData, _requiredQuantity);

                if (isEnoughProducts)
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity ok");

                    // 1 prepare prods
                    var (preparedProducts, maxLoads) =
                        WarehouseService.GetProductsForProductionAndMaxLoads(
                            _warehouseData,
                            _requiredQuantity,
                            aspect.BuildingData.LoadCapacity);

                    aspect.SetPreparedProductsToProduction(preparedProducts);
                    aspect.SetMaxLoads(maxLoads);

                    foreach (var q in preparedProducts)
                    {
                        Debug.LogWarning($"prepared: {q.Name} / {q.Quantity}");
                    }

                    // 2 reduce in warehouse
                    StorageService.ChangeProductsQuantity(_warehouseData.Value, Operation.Reduce, preparedProducts);

                    // 3 increase in productin box
                    StorageService.ChangeProductsQuantity(_productionBoxData.Value, Operation.Increase, preparedProducts);

                    // 3.1 update ui
                    // ecb.AddComponent<WarehouseDataUpdatedEvent>(aspect.BuildingData.Self);
                    ecb.AddComponent<InProductionDataUpdatedEvent>(aspect.BuildingData.Self);

                    // 4 start production
                    aspect.SetProductionState(ProductionState.EnoughProducts);

                    // 5 production settings to building SO
                }
                else
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity NOT ok");
                }

                ecb.RemoveComponent<ProductsToProductionBoxRequestTag>(aspect.BuildingData.Self);
            }
        }
    }
}