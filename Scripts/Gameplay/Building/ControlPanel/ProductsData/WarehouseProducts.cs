using System;
using JetBrains.Annotations;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.Gameplay.Building.ControlPanel.ProductsData
{
    /// <summary>
    /// Contains a hash map of products stored in the building's warehouse
    /// <para>HashMap of (int <see cref="Product"/> id, int quantity of product)</para>
    /// </summary>
    public struct WarehouseProducts : IBuildingProductsData
    {
        /// <summary>
        /// Hash map (int <see cref="Product"/> id, int quantity of product)
        /// </summary>
        public NativeParallelHashMap<int, int> Value;

        public int GetProductQuantity(Product product)
        {
            throw new System.NotImplementedException();
        }

        public bool IsEnoughRequiredProductsInStorage(NativeList<ProductData> requiredData)
        {
            bool first = Value[(int)requiredData[0].Name] >= requiredData[0].Quantity;

            //TODO LOOK Refactor this 
            return requiredData.Length switch
            {
                0 => throw new Exception("Building without requirements!!! OMG!!!"),
                1 => first,
                2 => first && Value[(int)requiredData[1].Name] >= requiredData[1].Quantity,
                _ => false
            };
        }

        public void SetProductsList(NativeParallelHashMap<int, int> productsMap) => Value = productsMap;

        public NativeList<ProductData> GetProductsDataList()
        {
            var a = new NativeList<ProductData>(0, Allocator.Temp);
            foreach (var value in Value)
            {
                a.Add(new ProductData
                {
                    Name = (Product)value.Key,
                    Quantity = value.Value
                });
            }

            return a;
        }

        // TODO warehouse capacity, stack capacity
        public void IncreaseProductsQuantity(NativeList<ProductData> productsData)
        {
            foreach (var product in productsData)
            {
                Value[(int)product.Name] += product.Quantity;
            }
        }

        public void SetValues(NativeParallelHashMap<int, int> valuesHashMap)
        {
            foreach (var product in valuesHashMap)
            {
                Value[product.Key] = product.Value;
            }
        }

        public NativeList<ProductData> GetPreparedProductsForProduction(NativeList<ProductData> requiredQuantity,
            int loadCapacity)
        {
            var preparedProducts = new NativeList<ProductData>(0, Allocator.Temp);
            var a = 0;

            foreach (var q in requiredQuantity)
            {
                a += q.Quantity;
            }

            if (a == 0) throw new Exception("Quantity 0.");

            int tempLoadsCount = (int)math.floor(loadCapacity / a);

            int maxLoads = GetMaxLoadsBasedOnAvailableProductsInWarehouse(tempLoadsCount, requiredQuantity);

            Debug.LogWarning("max load times = " + maxLoads);

            foreach (var q in requiredQuantity)
            {
                preparedProducts.Add(new ProductData
                {
                    Name = q.Name,
                    Quantity = q.Quantity * maxLoads
                });
            }

            return preparedProducts;
        }

        private int GetMaxLoadsBasedOnAvailableProductsInWarehouse(int maxLoads,
            NativeList<ProductData> requiredQuantity)
        {
            var tempMaxLoads = maxLoads;

            bool isFirstProductHasSufficientQuantity = Value[(int)requiredQuantity[0].Name] <
                                                       requiredQuantity[0].Quantity * maxLoads;

            switch (Value.Count())
            {
                case 1:
                    if (!isFirstProductHasSufficientQuantity) break;

                    tempMaxLoads = GetMaxLoadsBasedOnAvailableProductsInWarehouse(maxLoads - 1, requiredQuantity);

                    break;
                case 2:
                    bool isSecondProductHasSufficientQuantity = Value[(int)requiredQuantity[1].Name] <
                                                                requiredQuantity[1].Quantity * maxLoads;

                    if (!isFirstProductHasSufficientQuantity || !isSecondProductHasSufficientQuantity) break;

                    tempMaxLoads = GetMaxLoadsBasedOnAvailableProductsInWarehouse(maxLoads - 1, requiredQuantity);

                    break;
            }

            return tempMaxLoads;
        }
    }
}