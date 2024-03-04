using Sources.Scripts.CommonComponents.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonComponents.test
{
    /// <summary>
    /// Required and manufactured
    /// </summary>
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}