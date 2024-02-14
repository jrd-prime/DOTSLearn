using System;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Products.Component;
using Jrd.Gameplay.Storage._2_Warehouse;
using Jrd.Gameplay.Storage._2_Warehouse.Component;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Jrd.Gameplay.Storage.Service
{
    public class WarehouseService : StorageService
    {
        public static WarehouseService Instance { private set; get; }

        protected void Awake()
        {
            Instance ??= this;
        }

        public static bool IsEnoughRequiredProducts(WarehouseData warehouseData,
            NativeList<ProductData> requiredData)
        {
            bool first = warehouseData.Value[(int)requiredData[0].Name] >= requiredData[0].Quantity;
            Debug.LogWarning("in ");
            //TODO LOOK Refactor this 
            return requiredData.Length switch
            {
                0 => throw new Exception("Building without requirements!!! OMG!!!"),
                1 => first,
                2 => first && warehouseData.Value[(int)requiredData[1].Name] >= requiredData[1].Quantity,
                _ => false
            };
        }

        public static NativeList<ProductData> GetProductsForProduction(
            WarehouseData warehouseData, NativeList<ProductData> requiredQuantity,
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

            int maxLoads = GetMaxLoads(warehouseData.Value, tempLoadsCount, requiredQuantity);

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

        /// <summary>
        /// Return max possible loads based on available products in warehouse
        /// </summary>
        private static int GetMaxLoads(
            NativeParallelHashMap<int, int> warehouseData, int maxLoads,
            NativeList<ProductData> requiredQuantity)
        {
            var tempMaxLoads = maxLoads;

            bool isFirstProductHasSufficientQuantity =
                warehouseData[(int)requiredQuantity[0].Name] < requiredQuantity[0].Quantity * maxLoads;

            switch (warehouseData.Count())
            {
                case 1:
                    if (!isFirstProductHasSufficientQuantity) break;

                    tempMaxLoads = GetMaxLoads(warehouseData, maxLoads - 1, requiredQuantity);
                    break;
                case 2:
                    bool isSecondProductHasSufficientQuantity =
                        warehouseData[(int)requiredQuantity[1].Name] < requiredQuantity[1].Quantity * maxLoads;

                    if (!isFirstProductHasSufficientQuantity || !isSecondProductHasSufficientQuantity) break;

                    tempMaxLoads = GetMaxLoads(warehouseData, maxLoads - 1, requiredQuantity);
                    break;
            }

            return tempMaxLoads;
        }
    }
}