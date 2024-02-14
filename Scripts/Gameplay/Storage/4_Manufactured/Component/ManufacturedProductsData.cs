using Jrd.Gameplay.Products.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage._4_Manufactured.Component
{
    /// <summary>
    /// Required and manufactured
    /// </summary>
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Manufactured;
    }
}