using Jrd.Gameplay.Building.ControlPanel;
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
        /// <summary>
        /// Increased here <see cref="BuildingControlPanelSystem"/> after one load timer finished
        /// </summary>
        public int CurrentCycle;
        public bool LastCycleEnd;
    }
}