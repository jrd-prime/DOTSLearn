using Sources.Scripts.CommonComponents.Product;
using Sources.Scripts.Game.Features.Building.ControlPanel;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production.Component
{
    public struct ProductionProcessData : IComponentData
    {
        public int MaxLoads;
        public NativeList<ProductData> PreparedProducts;
        public float AllProductsTimer;
        public float OneProductTimer;
        /// <summary>
        /// Increased here <see cref="BuildingControlPanelSystem"/> after one load timer finished
        /// </summary>
        public int CurrentCycle;
        public bool LastCycleEnd;
    }
}