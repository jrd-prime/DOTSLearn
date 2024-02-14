﻿using Jrd.Gameplay.Building.ControlPanel.Component;
using Jrd.Gameplay.Building.Production;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage._4_Manufactured.Component;
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
        private readonly RefRO<BuildingProductsData> _buildingProductsData;
        private readonly RefRO<RequiredProductsData> _requiredProductsData;
        private readonly RefRO<ManufacturedProductsData> _manufacturedProductsData;

        public Entity Self => _self;
        public BuildingData BuildingData => _buildingData.ValueRO;
        public BuildingProductsData BuildingProductsData => _buildingProductsData.ValueRO;
        public RequiredProductsData RequiredProductsData => _requiredProductsData.ValueRO;
        public ManufacturedProductsData ManufacturedProductsData => _manufacturedProductsData.ValueRO;


        public void SetProductionState(ProductionState value) => _buildingData.ValueRW.ProductionState = value;
    }
}