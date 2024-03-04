using Sources.Scripts.CommonData.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Storage.Data
{
    public struct ProductsToDeliveryData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}