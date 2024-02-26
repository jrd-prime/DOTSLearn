﻿using CommonComponents.Product;
using GamePlay.Features.Building.Events;
using GamePlay.Features.Building.Production;
using GamePlay.Features.Building.Products.Component;
using GamePlay.Features.Building.Storage.InProductionBox.Component;
using GamePlay.Features.Building.Storage.Service;
using GamePlay.Features.Building.Storage.Warehouse.Component;
using GamePlay.UI;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace GamePlay.Features.Building.Storage.InProductionBox
{
    /// <summary>
    /// Move products from the building warehouse to production box<br/>
    /// <see cref="MoveToProductionBoxRequestTag"/>
    /// </summary>
    public partial struct MoveToProductionBoxSystem : ISystem
    {
        private BuildingDataAspect _aspect;
        private WarehouseData _warehouseData;
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
                _warehouseData = aspect.BuildingProductsData.WarehouseData;
                _requiredQuantity = aspect.RequiredProductsData.Required;

                bool isEnoughProductsToLoad =
                    WarehouseService.IsEnoughRequiredProducts(_warehouseData, _requiredQuantity);

                if (isEnoughProductsToLoad)
                {
                    _preparedProducts = WarehouseService.GetProductsForProductionAndMaxLoads(
                        _warehouseData,
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