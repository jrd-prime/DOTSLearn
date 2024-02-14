using Jrd.Gameplay.Products;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Storage._4_Manufactured
{
    public struct ManufacturedProductsData : IComponentData
    {
        public NativeList<ProductData> Manufactured;
    }
}