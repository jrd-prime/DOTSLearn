using System;
using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.Component
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