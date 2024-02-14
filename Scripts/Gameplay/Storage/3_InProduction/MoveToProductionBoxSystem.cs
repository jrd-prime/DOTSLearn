using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.Production;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage._2_Warehouse.Component;
using Jrd.Gameplay.Storage._3_InProduction.Component;
using Jrd.Gameplay.Storage.Service;
using Jrd.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage._3_InProduction
{
    /// <summary>
    /// Move products from the building warehouse to production box<br/>
    /// <see cref="ProductsToProductionBoxRequestTag"/>
    /// </summary>
    public partial struct MoveToProductionBoxSystem : ISystem
    {
        private WarehouseData _warehouseData;
        private InProductionData _productionData;
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
                _productionData = aspect.BuildingProductsData.InProductionData;
                _requiredQuantity = aspect.RequiredProductsData.Required;

                bool isEnoughProducts =
                    WarehouseService.IsEnoughRequiredProducts(_warehouseData, _requiredQuantity);

                if (isEnoughProducts)
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity ok");

                    // 1 prepare prods
                    NativeList<ProductData> preparedProducts = WarehouseService.GetProductsForProduction(
                        _warehouseData,
                        _requiredQuantity,
                        aspect.BuildingData.LoadCapacity);


                    foreach (var q in preparedProducts)
                    {
                        Debug.LogWarning($"prepared: {q.Name} / {q.Quantity}");
                    }

                    // 2 reduce in warehouse
                    StorageService.ChangeProductsQuantity(_warehouseData.Value, Operation.Reduce, preparedProducts);

                    // 3 increase in productin box
                    StorageService.ChangeProductsQuantity(_productionData.Value, Operation.Increase, preparedProducts);

                    // 3.1 update ui
                    ecb.AddComponent<WarehouseDataUpdatedEvent>(aspect.BuildingData.Self);
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