using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.ControlPanel.ProductsData;
using Jrd.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Products
{
    /// <summary>
    /// Move products from the building warehouse to production box<br/>
    /// <see cref="ProductsToProductionBoxRequestTag"/>
    /// </summary>
    public partial struct MoveToProductionBoxSystem : ISystem
    {
        private WarehouseProducts _warehouseProducts;
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

                _warehouseProducts = aspect.BuildingProductsData.WarehouseProductsData;
                _requiredQuantity = aspect.RequiredProductsData.Required;

                bool isEnoughProducts = _warehouseProducts.IsEnoughRequiredProductsInStorage(_requiredQuantity);

                if (isEnoughProducts)
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity ok");


                    // 1 prepare prods
                    NativeList<ProductData> preparedProducts =
                        _warehouseProducts.GetPreparedProductsForProduction(_requiredQuantity,
                            aspect.BuildingData.LoadCapacity);

                    foreach (var q in preparedProducts)
                    {
                        Debug.LogWarning($"prepared: {q.Name} / {q.Quantity}");
                    }

                    // 2 reduce in warehouse


                    // 3 increase in productin box


                    // 4 start production


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