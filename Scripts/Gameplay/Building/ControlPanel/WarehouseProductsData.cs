using Jrd.Gameplay.Product;
using Jrd.Gameplay.Storage;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;

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
                a.Add(new ProductData
                {
                    Name = (Product.Product)value.Key,
                    Quantity = value.Value
                });
            }

            return a;
        }

        // TODO warehouse capacity, stack capacity
        public NativeList<ProductData> UpdateProductsCount(NativeList<ProductData> productsData)
        {
            var movedProductsList = new NativeList<ProductData>(0, Allocator.Temp);
            foreach (var productData in productsData)
            {
                Values[(int)productData.Name] += productData.Quantity;
                movedProductsList.Add(new ProductData
                {
                    Name = productData.Name,
                    Quantity = productData.Quantity
                });
            }

            return movedProductsList;
        }
    }
}