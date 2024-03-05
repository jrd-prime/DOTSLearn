using System;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Building
{
    public struct BuildingProductsData : IComponentData, IDisposable
    {
        public NativeList<ProductData> ProductsToDelivery;
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