using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Products.Component
{
    public struct ProductsToDeliveryData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}