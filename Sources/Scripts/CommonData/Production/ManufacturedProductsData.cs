using Sources.Scripts.CommonData.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Production
{
    /// <summary>
    /// Required and manufactured
    /// </summary>
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Value;
    }
}