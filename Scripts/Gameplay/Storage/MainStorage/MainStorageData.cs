using Jrd.Gameplay.Products;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    public struct MainStorageData : IComponentData, IMainStorage
    {
        public NativeParallelHashMap<int, int> Values;

        public int GetProductCount(Products.Product product) =>
            Values.ContainsKey((int)product) ? Values[(int)product] : -1;

        public NativeList<ProductData> GetMatchingProducts(NativeList<ProductionProductData> requiredItemsBuffer)
        {
            var productDataList = new NativeList<ProductData>(0, Allocator.Temp);

            for (var i = 0; i < requiredItemsBuffer.Length; i++)
            {
                Product product = requiredItemsBuffer[i]._productName;
                var key = (int)product;

                if (!Values.ContainsKey(key) && Values[key] < 0) continue;

                productDataList.Add(new ProductData
                {
                    Name = product,
                    Quantity = Values[key]
                });
            }

            return productDataList;
        }

        public void UpdateProductsByKey(NativeList<ProductData> productDatas)
        {
            foreach (var productData in productDatas)
            {
                Values[(int)productData.Name] -= productData.Quantity;
            }
        }

        public int GetProductsQuantity(NativeList<ProductData> productDataList)
        {
            var quantity = 0;
            foreach (ProductData product in productDataList)
            {
                quantity += product.Quantity;
            }

            return quantity;
        }
    }
}