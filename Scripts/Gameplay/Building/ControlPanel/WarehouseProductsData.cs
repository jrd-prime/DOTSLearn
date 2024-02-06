using Jrd.Gameplay.Product;
using Jrd.Gameplay.Storage;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Jrd.Gameplay.Building.ControlPanel
{
    /// <summary>
    /// Building warehouse products list component
    /// </summary>
    public struct WarehouseProductsData : IComponentData, IWarehouse
    {
        public NativeParallelHashMap<int, int> Values;

        public int GetProductCount(Product.Product product)
        {
            throw new System.NotImplementedException();
        }

        public NativeList<ProductData> GetProductsList(DynamicBuffer<BuildingRequiredItemsBuffer> buildingData)
        {
            var a = new NativeList<ProductData>(0, Allocator.Temp);

            foreach (var value in Values)
            {
                Debug.LogWarning($"add prod: {(Product.Product)value.Key} + {value.Value}");
                a.Add(new ProductData
                {
                    Name = (Product.Product)value.Key,
                    Quantity = value.Value
                });
            }

            return a;
        }
    }
}