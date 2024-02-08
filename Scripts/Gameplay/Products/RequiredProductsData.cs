using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Products
{
    /// <summary>
    /// Contains a list of <see cref="ProductionProductData"/> required for production in the building
    /// </summary>
    public struct RequiredProductsData : IComponentData
    {
        public NativeList<ProductionProductData> Required;
    }
}