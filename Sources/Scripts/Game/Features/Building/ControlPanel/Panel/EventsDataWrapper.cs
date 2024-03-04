using System;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Unity.Collections;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public struct EventsDataWrapper : IDisposable
    {
        public BuildingDataAspect Aspect;
        public MainStorageBoxData MainStorageBoxData;
        public NativeList<ProductData> ProductsToDelivery;

        public void Dispose()
        {
            Aspect.Dispose();
            MainStorageBoxData.Dispose();
            ProductsToDelivery.Dispose();
        }
    }
}