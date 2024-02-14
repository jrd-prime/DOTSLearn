using Unity.Collections;

namespace Jrd.Gameplay.Storage._3_InProduction.Component
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity) of the products in ready-to-process
    /// </summary>
    public struct InProductionData : IBuildingProductsData
    {
        public NativeParallelHashMap<int, int> Value;
    }
}