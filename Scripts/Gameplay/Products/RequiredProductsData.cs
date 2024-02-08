using Jrd.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Products
{
    public struct RequiredProductsData : IComponentData
    {
        public NativeList<ProductionProductData> Required;
    }
}