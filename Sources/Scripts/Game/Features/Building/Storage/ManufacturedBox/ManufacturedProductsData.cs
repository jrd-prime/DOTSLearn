using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Storage.ManufacturedBox
{
    /// <summary>
    /// Required and manufactured
    /// </summary>
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}