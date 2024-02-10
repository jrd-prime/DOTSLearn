using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building.ControlPanel
{
    /// <summary>
    /// Contains a hash map of products stored in the building's warehouse
    /// <para>HashMap of (int <see cref="Product"/> id, int quantity of product)</para>
    /// </summary>
    public struct WarehouseProductsData : IComponentData, IWarehouse
    {
        /// <summary>
        /// Hash map (int <see cref="Product"/> id, int quantity of product)
        /// </summary>
        public NativeParallelHashMap<int, int> Values;

        public int GetProductQuantity(Product product)
        {
            throw new System.NotImplementedException();
        }

        public NativeList<ProductData> GetProductsList(NativeList<ProductData> buildingData)
        {
            var a = new NativeList<ProductData>(0, Allocator.Temp);
            foreach (var value in Values)
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
        public NativeList<ProductData> UpdateProductsQuantity(NativeList<ProductData> productsData)
        {
            var movedProductsList = new NativeList<ProductData>(0, Allocator.Temp);
            foreach (var product in productsData)
            {
                Values[(int)product.Name] += product.Quantity;
                movedProductsList.Add(new ProductData
                {
                    Name = product.Name,
                    Quantity = product.Quantity
                });
            }

            return movedProductsList;
        }

        public void SetValues(NativeParallelHashMap<int, int> valuesHashMap)
        {
            foreach (var product in valuesHashMap)
            {
                Values[product.Key] = product.Value;
            }
        }
    }
}