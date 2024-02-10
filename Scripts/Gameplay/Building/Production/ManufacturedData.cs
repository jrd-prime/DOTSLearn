using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building.Production
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity)  of already produced products in the production box
    /// </summary>
    public struct ManufacturedData : IComponentData
    {
        public NativeParallelHashMap<int, int> Value;
    }
}