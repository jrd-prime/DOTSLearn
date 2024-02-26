using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage
{
    public struct ProductsToDeliveryData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}