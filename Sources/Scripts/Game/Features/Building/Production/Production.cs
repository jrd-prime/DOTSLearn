using System;
using System.Collections.Generic;
using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Production;
using Sources.Scripts.Game.Features.Building.Production.StateHandler;
using Unity.Assertions;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public unsafe struct Production
    {
        private Dictionary<ProductionState, IProductionStateProvider> _handlers;
        private ProductionMethods* _productionMethodsPtr;
        private bool _isInit;

        private void Init()
        {
            _handlers = new Dictionary<ProductionState, IProductionStateProvider>
            {
                { ProductionState.NotEnoughProducts, new NotEnough() },
                { ProductionState.EnoughProducts, new Enough() },
                { ProductionState.Init, new Initialize() },
                { ProductionState.Started, new Started() },
                { ProductionState.InProgress, new InProgress() },
                { ProductionState.Stopped, new Stopped() },
                { ProductionState.Finished, new Finished() },
            };

            ProductionMethods productionMethods = new();
            _productionMethodsPtr = &productionMethods;

            _isInit = true;
        }

        public void Process(ProductionState state, BuildingDataAspect aspect, EntityCommandBuffer ecb)
        {
            if (!_isInit) Init();

            Assert.IsTrue(Enum.GetNames(typeof(ProductionState)).Length == _handlers.Count,
                "The number of states does not correspond to the initialized states");

            _handlers[state].Run(
                new ProductionProcessDataWrapper
                {
                    Ecb = ecb,
                    Aspect = aspect,
                    ProductionMethodsPtr = _productionMethodsPtr
                });
        }
    }
}