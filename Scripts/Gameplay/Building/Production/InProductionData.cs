using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building.Production
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity) of the products in ready-to-process
    /// </summary>
    public struct InProductionData : IComponentData
    {
        public NativeParallelHashMap<int, int> Value;
    }
}