using System;
using Unity.Collections;

namespace Sources.Scripts.CommonComponents.Product
{
    [Serializable]
    public struct ProductData
    {
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
        /// Convert list <see cref="ProductData"/> to hashmap, set data quantity by <see cref="ProductValues"/>
        /// </summary>
        public static NativeParallelHashMap<int, int> ConvertProductsDataToHashMap(
            NativeList<ProductData> list, ProductValues values)
        {
            NativeParallelHashMap<int, int> map = new(list.Length, Allocator.Persistent);

            foreach (var product in list)
            {
                int quantity = values switch
                {
                    ProductValues.Keep => product._quantity,
                    ProductValues.ToDefault => 0,
                    _ => throw new ArgumentOutOfRangeException(nameof(values), values, null)
                };

                map.Add((int)product.Name, quantity);
            }

            return map;
        }

        public static void ConvertProductsHashMapToList(
            NativeParallelHashMap<int, int> map,
            out NativeList<ProductData> list)
        {
            list = new NativeList<ProductData>(map.Count(), Allocator.Persistent);

            if (map.IsEmpty) return;

            foreach (var product in map)
            {
                list.Add(new ProductData { Name = (Product)product.Key, Quantity = product.Value });
            }
        }
    }

    public enum ProductValues
    {
        Keep,
        ToDefault
    }
}