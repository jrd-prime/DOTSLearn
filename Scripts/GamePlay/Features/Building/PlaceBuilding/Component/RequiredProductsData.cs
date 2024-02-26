using System;
using CommonComponents;
using CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Features.Building.Products.Component
{
    /// <summary>
    /// Contains a list of <see cref="ProductData"/> required for production in the building
    /// </summary>
    [Serializable]
    public struct RequiredProductsData : IComponentData
    {
        public NativeList<ProductData> Required;
    }
}