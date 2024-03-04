using Sources.Scripts.CommonData.Product;
using Sources.Scripts.CommonData.Storage.Data;
using Unity.Collections;

namespace Sources.Scripts.CommonData.Storage
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