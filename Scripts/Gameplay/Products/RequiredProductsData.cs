using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

namespace Jrd.Gameplay.Products
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