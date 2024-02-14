using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Products.Component
{
    public struct ProductsToDeliveryData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}