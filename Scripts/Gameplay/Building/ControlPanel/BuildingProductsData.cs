using Jrd.Gameplay.Building.ControlPanel.ProductsData;
using Unity.Entities;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseProducts WarehouseProductsData;
        public InProductionProducts InProductionData;
        public ManufacturedProducts ManufacturedData;
    }
}