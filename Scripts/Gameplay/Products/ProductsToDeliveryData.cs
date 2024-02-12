using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Products
{
    public struct ProductsToDeliveryData : IComponentData
    {
        public NativeList<ProductData> Value;

        public void Dispose()
        {
            Value.Dispose();
        }
    }
}