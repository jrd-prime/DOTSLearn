using System;
using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Assert = Unity.Assertions.Assert;

namespace Sources.Scripts.CommonComponents.test.Service
{
    public abstract class StorageService
    {
        /// <summary>
        /// Get <see cref="Product"/> quantity
        /// </summary>
        /// <param name="product"><see cref="Product"/></param>
        /// <param name="value"> NativeParallelHashMap(int, int)</param>
        /// <returns>Quantity of product <b>OR</b> <b>-1</b> if product not yet available (because there are no buildings required for this product)</returns>
        public static int GetProductQuantity(NativeParallelHashMap<int, int> value, Product.Product product) =>
            value.ContainsKey((int)product) ? value[(int)product] : -1;


        public static void ChangeProductsQuantity(NativeParallelHashMap<int, int> inProductionProducts,
            Operation operation,
            NativeList<ProductData> productsData)
        {
            switch (operation)
            {
                case Operation.Increase:
                    foreach (var q in productsData)
                    {
                        inProductionProducts[(int)q.Name] += q.Quantity;
                    }

                    break;
                case Operation.Reduce:
                    foreach (var q in productsData)
                    {
                        inProductionProducts[(int)q.Name] -= q.Quantity;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }

        public static NativeList<ProductData> GetProductsDataList(NativeParallelHashMap<int, int> warehouseProductsData)
        {
            var productDataList = new NativeList<ProductData>(0, Allocator.Persistent);

            foreach (var product in warehouseProductsData)
            {
                productDataList.Add(new ProductData { Name = (Product.Product)product.Key, Quantity = product.Value });
            }

            return productDataList;
        }

        /// <summary>
        /// Matching products in <see cref="MainStorageData"/>
        /// <param name="requested">list of <see cref="ProductData"/></param>
        /// <returns>list of <see cref="ProductData"/></returns>
        /// </summary>
        public static NativeList<ProductData> GetMatchingProducts(NativeList<ProductData> requested,
            NativeParallelHashMap<int, int> storageData, out bool isEnough)
        {
            var matchingProducts = new NativeList<ProductData>(0, Allocator.Persistent);

            for (var i = 0; i < requested.Length; i++)
            {
                Product.Product product = requested[i].Name;

                Assert.IsTrue(storageData.ContainsKey((int)product), $"Storage key {(int)product} not exist");

                matchingProducts.Add(new ProductData
                {
                    Name = product,
                    Quantity = storageData[(int)product]
                });
            }

            isEnough = requested.Length switch
            {
                0 => false,
                1 => storageData[(int)requested[0].Name] > 0,
                2 => storageData[(int)requested[0].Name] > 0 ||
                     storageData[(int)requested[1].Name] > 0,
                _ => false
            };

            return matchingProducts;
        }

        /// <summary>
        /// Reduce products quantity in <see cref="MainStorageData"/> by key-value from list of <see cref="ProductData"/>
        /// <param name="productsData">list of <see cref="ProductData"/></param>
        /// </summary>
        public static void ReduceProductsQuantityByKey(NativeParallelHashMap<int, int> value,
            NativeList<ProductData> productsData)
        {
            foreach (var productData in productsData)
            {
                value[(int)productData.Name] -= productData.Quantity;
            }
        }

        /// <summary>
        /// Increase products quantity in <see cref="MainStorageData"/> by key-value from list of <see cref="ProductData"/>
        /// <param name="productsData">list of <see cref="ProductData"/></param>
        /// </summary>
        public static void IncreaseProductsQuantityByKey(NativeParallelHashMap<int, int> value,
            NativeList<ProductData> productsData)
        {
            foreach (var productData in productsData)
            {
                value[(int)productData.Name] += productData.Quantity;
            }
        }

        /// <summary>
        /// Get sum of products quantity in <see cref="MainStorageData"/> by key-value from list of <see cref="ProductData"/>
        /// <param name="productsData">list of <see cref="ProductData"/></param>
        /// <returns>Sum of products quantity</returns>
        /// </summary>
        public static int GetProductsQuantity(NativeList<ProductData> productsData)
        {
            var quantity = 0;

            foreach (var product in productsData)
            {
                quantity += product.Quantity;
            }

            return quantity;
        }
    }

    public enum Operation
    {
        Increase,
        Reduce
    }
}