using Jrd.Gameplay.Storage.InProductionBox.Component;
using Jrd.Gameplay.Storage.ManufacturedBox;
using Jrd.Gameplay.Storage.Warehouse.Component;
using Unity.Entities;

namespace Jrd.Gameplay.Building.ControlPanel.Component
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseData WarehouseData;
        public InProductionBoxData InProductionBoxData;
        public ManufacturedBoxData ManufacturedBoxData;
    }
}