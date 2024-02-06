using Jrd.Gameplay.Product;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    public struct MainStorageData : IComponentData, IMainStorage
    {
        public NativeParallelHashMap<int, int> Values;

        public int GetProductCount(Product.Product product) => Values.ContainsKey((int)product) ? Values[(int)product] : -1;

        public NativeList<ProductData> GetMatchingProducts(
            DynamicBuffer<BuildingRequiredItemsBuffer> requiredItemsBuffer)
        {
            var productDataList = new NativeList<ProductData>(0, Allocator.Temp);

            for (var i = 0; i < requiredItemsBuffer.Length; i++)
            {
                Product.Product product = requiredItemsBuffer[i]._item;
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
    }
}