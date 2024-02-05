using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.Goods;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Storage.MainStorage
{
    public struct MainStorageData : IComponentData, IDB
    {
        public NativeParallelHashMap<GoodsEnum, int> Values;

        public int GetProductCount(GoodsEnum product)
        {
            FixedString32Bytes key = nameof(product);

            return Values.ContainsKey(key) ? Values[key] : -1;
        }

        public bool GetMatchingProducts(DynamicBuffer<BuildingRequiredItemsBuffer> requiredItemsBuffer,
            out NativeList<ProductData> productDataList)
        {
            foreach (var q in requiredItemsBuffer)
            {
                Debug.LogWarning(q._item);
            }
            
            productDataList = new NativeList<ProductData>(0, Allocator.Temp);

            for (var i = 0; i < requiredItemsBuffer.Length; i++)
            {
                GoodsEnum product = requiredItemsBuffer[i]._item;
                string productName = product.ToString();

                if (!Values.ContainsKey(productName)) continue;
                if (Values[productName] != -1)
                {
                    productDataList.Add(new ProductData
                    {
                        Name = product,
                        Quantity = Values[productName]
                    });
                }
            }

            return !productDataList.IsEmpty && productDataList.Length != 0;
        }
    }

    public interface IDB
    {
        public int GetProductCount(GoodsEnum product);
    }
}