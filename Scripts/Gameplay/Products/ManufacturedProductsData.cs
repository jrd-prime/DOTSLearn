using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Products
{
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}