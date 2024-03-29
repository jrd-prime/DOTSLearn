﻿using Sources.Scripts.CommonData.Product;
using Unity.Collections;
using Unity.Entities;

namespace Sources.Scripts.CommonData.Production
{
    public struct ProductionProcessData : IComponentData
    {
        public bool IsRunning;
        public int MaxLoads;
        public NativeList<ProductData> PreparedProducts;
        public float AllProductsTimer;
        public float OneProductTimer;
        public int CycleInProgress;

        /// <summary>
        /// Increased here <see cref="ControlPanel.System.BuildingControlPanelSystem"/> after one load timer finished
        /// </summary>
        public int CurrentCycle;

        public bool LastCycleEnd;
    }
}