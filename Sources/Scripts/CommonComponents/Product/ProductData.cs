using System;
using Unity.Collections;

namespace Sources.Scripts.CommonComponents.Product
{
    [Serializable]
    public struct ProductData
    {
        private Product _name;
        private int _quantity;
        private float _moveTimeMultiplier;

        public Product Name { get; set; }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value >= 0) _quantity = value;
            }
        }

        public float MoveTimeMultiplier
        {
            get => _moveTimeMultiplier;
            set
            {
                if (value >= 0) _moveTimeMultiplier = value;
            }
        }


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
                    ProductValues.Keep => product._quantity,
                    ProductValues.ToDefault => 0,
                    _ => throw new ArgumentOutOfRangeException(nameof(values), values, null)
                };

                nativeParallelHashMap.Add((int)product._name, quantity);
            }

            return nativeParallelHashMap;
        }

        public static void ConvertProductsHashMapToList(
            NativeParallelHashMap<int, int> inputData,
            out NativeList<ProductData> outputData)
        {
            outputData = new NativeList<ProductData>(inputData.Count(), Allocator.Persistent);

            if (inputData.IsEmpty) return;

            foreach (var product in inputData)
            {
                outputData.Add(new ProductData { _name = (Product)product.Key, _quantity = product.Value });
            }
        }
    }

    public enum ProductValues
    {
        Keep,
        ToDefault
    }
}