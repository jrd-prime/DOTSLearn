﻿using System;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Production;
using Sources.Scripts.CommonData.Storage;
using Sources.Scripts.CommonData.Storage.Data;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Building
{
    /// <summary>
    /// Building Data Aspect
    /// <para>
    /// <see cref="BuildingData"/><br />
    /// <see cref="ProductsInBuildingData"/><br />
    /// <see cref="RequiredProductsData"/><br />
    /// <see cref="ManufacturedProductsData"/>
    /// </para>
    /// </summary>
    public readonly partial struct BuildingDataAspect : IAspect, IDisposable
    {
        private readonly Entity _self;
        private readonly RefRW<BuildingData> _buildingData;
        private readonly RefRW<BuildingProductsData> _buildingProductsData;
        private readonly RefRW<RequiredProductsData> _requiredProductsData;
        private readonly RefRW<ManufacturedProductsData> _manufacturedProductsData;
        private readonly RefRW<ProductionProcessData> _productionProcessData;
        private readonly RefRW<ChangeProductsQuantityQueueData> _changeProductsQuantityData;

        public Entity Self => _self;
        public BuildingData BuildingData => _buildingData.ValueRO;
        public BuildingProductsData ProductsInBuildingData => _buildingProductsData.ValueRW;
        public RequiredProductsData RequiredProductsData => _requiredProductsData.ValueRO;
        public ManufacturedProductsData ManufacturedProductsData => _manufacturedProductsData.ValueRO;
        public RefRW<ProductionProcessData> ProductionProcessData => _productionProcessData;
        public ChangeProductsQuantityQueueData ChangeProductsQuantityData => _changeProductsQuantityData.ValueRW;


        public void SetProductionState(ProductionState value) => _buildingData.ValueRW.ProductionState = value;

        // Production Process
        public void SetMaxLoads(int value) =>
            _productionProcessData.ValueRW.MaxLoads = value;

        public void SetPreparedProductsToProduction(NativeList<ProductData> value) =>
            _productionProcessData.ValueRW.PreparedProducts = value;

        public int GetOneProductManufacturingTime() =>
            (int)(60 * 60 / _buildingData.ValueRO.ItemsPerHour);

        public int GetLoadedProductsManufacturingTime() =>
            _productionProcessData.ValueRO.MaxLoads * GetOneProductManufacturingTime();

        public void SetFullLoadedProductsTimer() =>
            _productionProcessData.ValueRW.AllProductsTimer = GetLoadedProductsManufacturingTime();

        public void SetOneProductTimer() =>
            _productionProcessData.ValueRW.OneProductTimer = GetOneProductManufacturingTime();

        // Events
        public void AddEvent(BuildingEvent value) =>
            _buildingData.ValueRW.BuildingEvents.Enqueue(value);

        // Prods
        public void ChangeProductsQuantity(ChangeProductsQuantityData data) =>
            _changeProductsQuantityData.ValueRW.Value.Enqueue(data);

        public void SetCurrentCycle(int value) =>
            _productionProcessData.ValueRW.CurrentCycle = value;

        // TODO where to place a call?
        public void Dispose()
        {
            _buildingData.ValueRW.BuildingEvents.Dispose();
            _buildingProductsData.ValueRW.Dispose();
            _requiredProductsData.ValueRW.Value.Dispose();
            _manufacturedProductsData.ValueRW.Value.Dispose();
            _productionProcessData.ValueRW.PreparedProducts.Dispose();
            _changeProductsQuantityData.ValueRW.Value.Dispose();
        }

        public void SetProductsToDelivery(NativeList<ProductData> matchingProducts)
        {
            _buildingProductsData.ValueRW.ProductsToDelivery = matchingProducts;
        }
    }
}