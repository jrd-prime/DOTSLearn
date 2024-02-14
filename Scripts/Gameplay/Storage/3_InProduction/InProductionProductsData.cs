using Unity.Collections;

namespace Jrd.Gameplay.Storage._3_InProduction
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity) of the products in ready-to-process
    /// </summary>
    public struct InProductionProductsData : IBuildingProductsData
    {
        public NativeParallelHashMap<int, int> Value;
    }
}