using System.Threading.Tasks;
using Jrd.Gameplay.Products;
using Jrd.Gameplay.Storage;
using Unity.Collections;
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
        public void UpdateProductsQuantity(NativeList<ProductData> productsData)
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
    }
}