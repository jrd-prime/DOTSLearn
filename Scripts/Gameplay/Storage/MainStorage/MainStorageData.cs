using Jrd.Gameplay.Products;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage.MainStorage
{
    public struct MainStorageData : IComponentData, IMainStorage
    {
        /// <summary>
        /// Hash map (int <see cref="Product"/> id, int quantity of product)
        /// </summary>
        public NativeParallelHashMap<int, int> Values;

        /// <summary>
        /// Get <see cref="Product"/> quantity
        /// </summary>
        /// <param name="product"><see cref="Product"/></param>
        /// <returns>Quantity of product <b>OR</b> <b>-1</b> if product not yet available (because there are no buildings required for this product)</returns>
        public int GetProductQuantity(Product product) => Values.ContainsKey((int)product) ? Values[(int)product] : -1;

        /// <summary>
        /// Matching products in <see cref="MainStorageData"/> for building WarehouseProductsData
        /// <param name="requiredItemsList">list of <see cref="ProductData"/></param>
        /// <returns>list of <see cref="ProductData"/></returns>
        /// </summary>
        public NativeList<ProductData> GetMatchingProducts(NativeList<ProductData> requiredItemsList)
        {
            var productDataList = new NativeList<ProductData>(0, Allocator.Temp);

            for (var i = 0; i < requiredItemsList.Length; i++)
            {
                Product product = requiredItemsList[i].Name;
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

        /// <summary>
        /// Reduce products quantity in <see cref="MainStorageData"/> by key-value from list of <see cref="ProductData"/>
        /// <param name="productsData">list of <see cref="ProductData"/></param>
        /// </summary>
        public void ReduceProductsQuantityByKey(NativeList<ProductData> productsData)
        {
            foreach (var productData in productsData)
            {
                Values[(int)productData.Name] -= productData.Quantity;
            }
        }

        /// <summary>
        /// Increase products quantity in <see cref="MainStorageData"/> by key-value from list of <see cref="ProductData"/>
        /// <param name="productsData">list of <see cref="ProductData"/></param>
        /// </summary>
        public void IncreaseProductsQuantityByKey(NativeList<ProductData> productsData)
        {
            foreach (var productData in productsData)
            {
                Values[(int)productData.Name] += productData.Quantity;
            }
        }

        /// <summary>
        /// Get sum of products quantity in <see cref="MainStorageData"/> by key-value from list of <see cref="ProductData"/>
        /// <param name="productsData">list of <see cref="ProductData"/></param>
        /// <returns>Sum of products quantity</returns>
        /// </summary>
        public int GetProductsQuantity(NativeList<ProductData> productsData)
        {
            var quantity = 0;
            foreach (ProductData product in productsData)
            {
                quantity += product.Quantity;
            }

            return quantity;
        }
    }
}