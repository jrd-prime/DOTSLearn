using Jrd.Gameplay.Storage._2_Warehouse.Component;
using Jrd.Gameplay.Storage._3_InProduction.Component;
using Jrd.Gameplay.Storage._4_Manufactured;
using Unity.Entities;

namespace Jrd.Gameplay.Building.ControlPanel.Component
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseData WarehouseData;
        public InProductionData InProductionData;
        public ManufacturedData ManufacturedData;
    }
}