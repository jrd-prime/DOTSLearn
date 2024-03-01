using System;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.ControlPanel
{
    public struct BuildingProductsData : IComponentData, IDisposable
    {
        public WarehouseData WarehouseData;
        public InProductionBoxData InProductionBoxData;
        public ManufacturedBoxData ManufacturedBoxData;

        public void Dispose()
        {
            WarehouseData.Value.Dispose();
            InProductionBoxData.Value.Dispose();
            ManufacturedBoxData.Value.Dispose();
        }
    }
}