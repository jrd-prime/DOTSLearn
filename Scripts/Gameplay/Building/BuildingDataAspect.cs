using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Products;
using Unity.Entities;

namespace Jrd.Gameplay.Building
{
    public readonly partial struct BuildingDataAspect : IAspect
    {
        private readonly RefRO<BuildingData> _buildingData;
        private readonly RefRO<BuildingProductsData> _buildingProductsData;
        private readonly RefRO<RequiredProductsData> _requiredProductsData;
        private readonly RefRO<ManufacturedProductsData> _manufacturedProductsData;

        public BuildingData BuildingData => _buildingData.ValueRO;
        public BuildingProductsData BuildingProductsData => _buildingProductsData.ValueRO;
        public RequiredProductsData RequiredProductsData => _requiredProductsData.ValueRO;
        public ManufacturedProductsData ManufacturedProductsData => _manufacturedProductsData.ValueRO;
    }
}