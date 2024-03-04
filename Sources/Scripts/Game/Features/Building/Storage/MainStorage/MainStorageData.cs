using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Storage.InProductionBox;
using Unity.Assertions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Storage.MainStorage
{
    public struct MainStorageData : IComponentData, IMainStorage, IDisposable
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

        /// <summary>
        /// Matching products in <see cref="MainStorageData"/>
        /// </summary>
        public bool TryGetMatchingProducts(NativeList<ProductData> requestedProducts,
            out NativeList<ProductData> matchingProducts)
        {
            matchingProducts = new NativeList<ProductData>(0, Allocator.Persistent);

            bool isEnough = requestedProducts.Length switch
            {
                0 => false,
                1 => Value[(int)requestedProducts[0].Name] > 0,
                2 => Value[(int)requestedProducts[0].Name] > 0 || Value[(int)requestedProducts[1].Name] > 0,
                _ => false
            };

            if (!isEnough) return false;

            for (var i = 0; i < requestedProducts.Length; i++)
            {
                Product product = requestedProducts[i].Name;

                Assert.IsTrue(Value.ContainsKey((int)product), $"Storage key {(int)product} not exist");

                matchingProducts.Add(new ProductData
                {
                    Name = product,
                    Quantity = Value[(int)product]
                });
            }

            return true;
        }

        public void Dispose()
        {
            Value.Dispose();
        }
    }
}