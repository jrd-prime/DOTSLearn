using System;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.test
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