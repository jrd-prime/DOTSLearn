using Jrd.Gameplay.Building.ControlPanel;
using Jrd.Gameplay.Product;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage
{
    public interface IStorage
    {
        /// <summary>
        /// Return quantity of a product in main storage
        /// </summary>
        /// <param name="product"></param>
        /// <returns>Product quantity OR -1 if no product</returns>
        public int GetProductCount(Product.Product product);
    }

    public interface IMainStorage : IStorage
    {
        public NativeList<ProductData> GetMatchingProducts(DynamicBuffer<BuildingRequiredItemsBuffer> requiredItemsBuffer);
    }

    public interface IWarehouse : IStorage
    {
        public NativeList<ProductData> GetProductsList(DynamicBuffer<BuildingRequiredItemsBuffer> buildingData);
    }
}