using System;
using GamePlay.Products;
using GamePlay.Products.Component;
using GamePlay.Storage.InProductionBox.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Storage.MainStorage.Component
{
    public struct MainStorageData : IComponentData, IMainStorage
    {
        /// <summary>
        /// Hash map (int <see cref="Product"/> id, int quantity of product)
        /// </summary>
        public NativeParallelHashMap<int, int> Value;

        // TODO repeated, needed refact
        public void ChangeProductsQuantity(ChangeType change, NativeList<ProductData> productsData)
        {
            foreach (var product in productsData)
            {
                ChangeProductQuantity(product, change);
            }
        }

        private void ChangeProductQuantity(ProductData product, ChangeType change)
        {
            // If product not present in main storage, add
            if (!Value.ContainsKey((int)product.Name)) Value.Add((int)product.Name, 0);

            switch (change)
            {
                case ChangeType.Increase:
                    Value[(int)product.Name] += product.Quantity;
                    break;
                case ChangeType.Reduce:
                    Value[(int)product.Name] -= product.Quantity;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(change), change, null);
            }
        }
    }
}