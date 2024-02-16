using Jrd.Gameplay.Products.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.InProductionBox.Component
{
    public struct ChangeProductsQuantityData : IComponentData
    {
        public StorageType StorageType;
        public ChangeType ChangeType;
        public NativeList<ProductData> ProductsData;
    }

    public enum StorageType
    {
        Main,
        Warehouse,
        InProduction,
        Manufactured
    }

    public enum ChangeType
    {
        Increase,
        Reduce
    }
}