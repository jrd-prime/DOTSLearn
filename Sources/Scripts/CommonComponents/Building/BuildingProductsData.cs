using System;
using Sources.Scripts.CommonComponents.Storage;
using Sources.Scripts.CommonComponents.Storage.Data;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Building
{
    public struct BuildingProductsData : IComponentData, IDisposable
    {
        public WarehouseBoxData WarehouseBoxData;
        public InProductionBoxData InProductionBoxData;
        public ManufacturedBoxData ManufacturedBoxData;

        public void Dispose()
        {
            WarehouseBoxData.Value.Dispose();
            InProductionBoxData.Value.Dispose();
            ManufacturedBoxData.Value.Dispose();
        }
    }
}