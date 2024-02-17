using Jrd.Gameplay.Products.Component;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.Gameplay.Building.Production.Component
{
    public struct ProductionProcessData : IComponentData
    {
        public int MaxLoads;
        public NativeList<ProductData> PreparedProducts;
        public float AllProductsTimer;
        public float OneProductTimer;
        public int RemainingCycles;
        public int CurrentCycle;
    }
}