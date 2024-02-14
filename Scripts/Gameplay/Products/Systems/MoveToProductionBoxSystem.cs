﻿using Jrd.Gameplay.Building;
using Jrd.Gameplay.Storage;
using Jrd.Gameplay.Storage._2_Warehouse;
using Jrd.Gameplay.Storage._3_InProduction;
using Jrd.Gameplay.Storage.Service;
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
        private WarehouseProductsData _warehouseData;
        private InProductionProductsData _productionData;
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

                _warehouseData = aspect.BuildingProductsData.WarehouseProductsData;
                _productionData = aspect.BuildingProductsData.InProductionData;
                _requiredQuantity = aspect.RequiredProductsData.Required;

                WarehouseService warehouseService = WarehouseService.Instance;

                bool isEnoughProducts =
                    warehouseService.IsEnoughRequiredProducts(_warehouseData, _requiredQuantity);

                if (isEnoughProducts)
                {
                    TextPopUpMono.Instance.ShowPopUp("quantity ok");


                    // 1 prepare prods
                    NativeList<ProductData> preparedProducts = warehouseService
                        .GetProductsForProduction(_warehouseData, _requiredQuantity, aspect.BuildingData.LoadCapacity);


                    foreach (var q in preparedProducts)
                    {
                        Debug.LogWarning($"prepared: {q.Name} / {q.Quantity}");
                    }

                    // 2 reduce in warehouse
                    StorageService.ChangeProductsQuantity(_warehouseData, Operation.Reduce, preparedProducts);

                    // 3 increase in productin box
                    StorageService.ChangeProductsQuantity(_productionData, Operation.Increase, preparedProducts);

                    // 3.1 update ui
                    
                    
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