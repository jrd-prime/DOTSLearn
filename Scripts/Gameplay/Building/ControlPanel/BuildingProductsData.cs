using Jrd.Gameplay.Storage._2_Warehouse;
using Jrd.Gameplay.Storage._3_InProduction;
using Jrd.Gameplay.Storage._4_Manufactured;
using Unity.Entities;

namespace Jrd.Gameplay.Building.ControlPanel
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseProductsData WarehouseProductsData;
        public InProductionProductsData InProductionData;
        public ManufacturedProducts ManufacturedData;
    }
}