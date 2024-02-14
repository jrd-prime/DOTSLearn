using Unity.Collections;

namespace Jrd.Gameplay.Storage._4_Manufactured
{
    /// <summary>
    /// Contains a hashmap (int, int) (Product id, quantity)  of already produced products in the production box
    /// </summary>
    public struct ManufacturedProducts : IBuildingProductsData
    {
        public NativeParallelHashMap<int, int> Value;
    }
}