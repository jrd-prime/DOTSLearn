using Jrd.Gameplay.Products;
using Unity.Collections;

namespace Jrd.Gameplay.Storage._2_Warehouse
{
    /// <summary>
    /// Contains a hash map of products stored in the building's warehouse
    /// <para>HashMap of (int <see cref="Product"/> id, int quantity of product)</para>
    /// </summary>
    public struct WarehouseProductsData : IBuildingProductsData
    {
        /// <summary>
        /// Hash map (int <see cref="Product"/> id, int quantity of product)
        /// </summary>
        public NativeParallelHashMap<int, int> Value;
    }
}