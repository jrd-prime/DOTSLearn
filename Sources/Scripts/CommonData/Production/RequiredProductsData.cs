using System;
using Sources.Scripts.CommonData.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Production
{
    /// <summary>
    /// Contains a list of <see cref="ProductData"/> required for production in the building
    /// </summary>
    [Serializable]
    public struct RequiredProductsData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}