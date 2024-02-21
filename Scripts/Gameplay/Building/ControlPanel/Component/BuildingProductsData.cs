using GamePlay.Storage.InProductionBox.Component;
using GamePlay.Storage.ManufacturedBox;
using GamePlay.Storage.Warehouse.Component;
using Unity.Entities;

namespace GamePlay.Building.ControlPanel.Component
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseData WarehouseData;
        public InProductionBoxData InProductionBoxData;
        public ManufacturedBoxData ManufacturedBoxData;
    }
}