using System;
using Unity.Collections;

namespace GamePlay.Products.Component
{
    [Serializable]
    public struct ProductData
    {
        public Product Name;
        public int Quantity;

        /// <summary>
        /// +- based on weight
        /// </summary>
        public float MoveTimeMultiplier;

        /// <summary>
        /// Convert list <see cref="ProductData"/> to hashmap, set data quantity to 0
        /// </summary>
        public static NativeParallelHashMap<int, int> ConvertProductsDataToHashMap(
            NativeList<ProductData> nativeList, ProductValues values)
        {
            NativeParallelHashMap<int, int> nativeParallelHashMap = new(nativeList.Length, Allocator.Persistent);

            foreach (var product in nativeList)
            {
                int quantity = values switch
                {
                    ProductValues.Keep => product.Quantity,
                    ProductValues.ToDefault => 0,
                    _ => throw new ArgumentOutOfRangeException(nameof(values), values, null)
                };

                nativeParallelHashMap.Add((int)product.Name, quantity);
            }

            return nativeParallelHashMap;
        }


        public static NativeList<ProductData> ConvertProductsHashMapToList(
            NativeParallelHashMap<int, int> nativeList)
        {
            NativeList<ProductData> nativeParallelHashMap = new(nativeList.Count(), Allocator.Persistent);

            foreach (var product in nativeList)
            {
                nativeParallelHashMap.Add(new ProductData
                {
                    Name = (Product)product.Key,
                    Quantity = product.Value
                });
            }

            return nativeParallelHashMap;
        }
    }

    public enum ProductValues
    {
        Keep,
        ToDefault
    }
}