using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.test;
using Sources.Scripts.Game.Features.Building.Storage.MainStorage;
using Unity.Collections;

namespace Sources.Scripts.Game.Features.Building.ControlPanel.Panel
{
    public struct EventsDataWrapper : IDisposable
    {
        public CommonComponents.test.BuildingDataAspect Aspect;
        public MainStorageData MainStorageData;
        public NativeList<ProductData> ProductsToDelivery;

        public void Dispose()
        {
            Aspect.Dispose();
            MainStorageData.Dispose();
            ProductsToDelivery.Dispose();
        }
    }
}