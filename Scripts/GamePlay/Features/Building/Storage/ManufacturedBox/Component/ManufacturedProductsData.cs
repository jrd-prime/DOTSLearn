using GamePlay.Features.Building.Products.Component;
using Unity.Collections;
using Unity.Entities;

namespace GamePlay.Features.Building.Storage.ManufacturedBox.Component
{
    /// <summary>
    /// Required and manufactured
    /// </summary>
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Manufactured;
    }
}