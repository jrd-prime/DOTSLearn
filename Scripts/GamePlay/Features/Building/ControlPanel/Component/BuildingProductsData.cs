using GamePlay.Features.Building.Storage.InProductionBox.Component;
using GamePlay.Features.Building.Storage.ManufacturedBox;
using GamePlay.Features.Building.Storage.Warehouse.Component;
using Unity.Entities;

namespace GamePlay.Features.Building.ControlPanel.Component
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseData WarehouseData;
        public InProductionBoxData InProductionBoxData;
        public ManufacturedBoxData ManufacturedBoxData;
    }
}