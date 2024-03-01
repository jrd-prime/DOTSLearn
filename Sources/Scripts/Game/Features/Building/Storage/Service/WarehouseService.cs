using System;
using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.Storage.Warehouse;
using Unity.Collections;
using Unity.Mathematics;

namespace Sources.Scripts.Game.Features.Building.Storage.Service
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
            bool first = warehouseData.Value[(int)requiredData[0]._name] >= requiredData[0]._quantity;

            //TODO LOOK Refactor this 
            return requiredData.Length switch
            {
                0 => throw new Exception("Building without requirements!!! OMG!!!"),
                1 => first,
                2 => first && warehouseData.Value[(int)requiredData[1]._name] >= requiredData[1]._quantity,
                _ => false
            };
        }

        public static NativeList<ProductData> GetProductsForProductionAndMaxLoads(WarehouseData warehouseData,
            NativeList<ProductData> requiredQuantity,
            int loadCapacity, out int maxLoads)
        {
            var preparedProducts = new NativeList<ProductData>(0, Allocator.Persistent);
            var a = 0;

            foreach (var q in requiredQuantity)
            {
                a += q._quantity;
            }

            if (a == 0) throw new Exception("Quantity 0.");

            int tempLoadsCount = (int)math.floor(loadCapacity / a);

            maxLoads = GetMaxLoads(warehouseData.Value, tempLoadsCount, requiredQuantity);

            foreach (var q in requiredQuantity)
            {
                preparedProducts.Add(new ProductData
                {
                    _name = q._name,
                    _quantity = q._quantity * maxLoads
                });
            }

            return preparedProducts;
        }

        /// <summary>
        /// Return max possible loads based on available products in warehouse
        /// </summary>
        private static int GetMaxLoads(
            NativeParallelHashMap<int, int> warehouseData, int maxLoads, NativeList<ProductData> requiredQuantity)
        {
            var tempMaxLoads = maxLoads;

            bool isFirstHasSufficientQuantity =
                warehouseData[(int)requiredQuantity[0]._name] >= requiredQuantity[0]._quantity * maxLoads;

            switch (warehouseData.Count())
            {
                case 1:
                    if (isFirstHasSufficientQuantity) return tempMaxLoads;

                    tempMaxLoads = GetMaxLoads(warehouseData, maxLoads - 1, requiredQuantity);
                    break;
                case 2:
                    bool isSecondHasSufficientQuantity =
                        warehouseData[(int)requiredQuantity[1]._name] >= requiredQuantity[1]._quantity * maxLoads;

                    if (isFirstHasSufficientQuantity && isSecondHasSufficientQuantity) return tempMaxLoads;

                    tempMaxLoads = GetMaxLoads(warehouseData, maxLoads - 1, requiredQuantity);
                    break;
            }

            return tempMaxLoads;
        }
    }
}