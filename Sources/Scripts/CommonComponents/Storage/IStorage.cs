using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.CommonComponents.Storage.Data;
using Unity.Collections;

namespace Sources.Scripts.CommonComponents.Storage
{
    public interface IStorage
    {
        public void ChangeProductsQuantity(ChangeType change, NativeList<ProductData> productsData);
    }

    public interface IMainStorage : IStorage
    {
    }

    public interface IWarehouse : IStorage
    {
    }

    public interface IBuildingProductsData : IStorage
    {
    }
}