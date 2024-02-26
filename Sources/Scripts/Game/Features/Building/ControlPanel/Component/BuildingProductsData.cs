using Sources.Scripts.Game.Features.Building.Storage.InProductionBox.Component;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse.Component;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Component
{
    public struct BuildingProductsData : IComponentData
    {
        public WarehouseData WarehouseData;
        public InProductionBoxData InProductionBoxData;
        public ManufacturedBoxData ManufacturedBoxData;
    }
}