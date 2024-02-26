using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage.InProductionBox.Component
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