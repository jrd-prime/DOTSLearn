using CommonComponents.Product;
using GamePlay.Features.Building.Products.Component;
using GamePlay.Features.Building.Storage.InProductionBox.Component;
using Unity.Collections;

namespace GamePlay.Features.Building.Storage
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