using CommonComponents;
using CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Features.Building.Products.Component
{
    public struct ProductsToDeliveryData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}