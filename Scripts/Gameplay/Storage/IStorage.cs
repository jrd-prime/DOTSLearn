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
        public int GetProductCount(Products.Product product);
    }

    public interface IMainStorage : IStorage
    {
        public NativeList<ProductData> GetMatchingProducts(NativeList<ProductionProductData> requiredItemsBuffer);
    }

    public interface IWarehouse : IStorage
    {
        public NativeList<ProductData> GetProductsList(NativeList<ProductionProductData> buildingData);
    }
}