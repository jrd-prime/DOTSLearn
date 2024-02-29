using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Unity.Collections;

namespace Sources.Scripts.Game.Features.Building.Storage
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