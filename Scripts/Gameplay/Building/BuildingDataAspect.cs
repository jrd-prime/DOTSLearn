using Jrd.Gameplay.Building.ControlPanel.Component;
using Jrd.Gameplay.Building.Production;
using Jrd.Gameplay.Building.Production.Component;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage;
using Jrd.Gameplay.Storage.ManufacturedBox.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building
{
    /// <summary>
    /// Building Data Aspect
    /// <para>
    /// <see cref="BuildingData"/><br />
    /// <see cref="BuildingProductsData"/><br />
    /// <see cref="RequiredProductsData"/><br />
    /// <see cref="ManufacturedProductsData"/>
    /// </para>
    /// </summary>
    public readonly partial struct BuildingDataAspect : IAspect
    {
        private readonly Entity _self;
        private readonly RefRW<BuildingData> _buildingData;
        private readonly RefRW<BuildingProductsData> _buildingProductsData;
        private readonly RefRO<RequiredProductsData> _requiredProductsData;
        private readonly RefRO<ManufacturedProductsData> _manufacturedProductsData;
        private readonly RefRW<ProductionProcessData> _productionProcessData;
        private readonly RefRW<ChangeProductsQuantityQueueData> _changeProductsQuantityData;

        public Entity Self => _self;
        public BuildingData BuildingData => _buildingData.ValueRO;
        public BuildingProductsData BuildingProductsData => _buildingProductsData.ValueRW;
        public RequiredProductsData RequiredProductsData => _requiredProductsData.ValueRO;
        public ManufacturedProductsData ManufacturedProductsData => _manufacturedProductsData.ValueRO;
        public ProductionProcessData ProductionProcessData => _productionProcessData.ValueRO;
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

        public void SetAllProductsTimer() =>
            _productionProcessData.ValueRW.AllProductsTimer = GetLoadedProductsManufacturingTime();

        public void SetOneProductTimer() =>
            _productionProcessData.ValueRW.OneProductTimer = GetOneProductManufacturingTime();
    }
}