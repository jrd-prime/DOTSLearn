using Jrd.Gameplay.Products;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.ScriptableObjects;
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
        public int GetProductQuantity(Products.Product product);
    }

    public interface IMainStorage : IStorage
    {
        public NativeList<ProductData> GetMatchingProducts(NativeList<ProductData> requiredItemsList, Allocator allocator);
    }

    public interface IWarehouse : IStorage
    {
        public NativeList<ProductData> GetProductsList(NativeList<ProductData> buildingData);
    }

    public interface IBuildingProductsData : IStorage
    {
        public void SetProductsList(NativeParallelHashMap<int, int> nativeParallelHashMap);
    }
}