using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage._3_InProduction.Component;
using Unity.Collections;

namespace Jrd.Gameplay.Storage
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