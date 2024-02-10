using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Building.Production;
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
    }

    public struct BuildingProductsData : IComponentData
    {
        public WarehouseProductsData WarehouseProductsData;
        public InProductionData InProductionData;
        public ManufacturedData ManufacturedData;
    }
}