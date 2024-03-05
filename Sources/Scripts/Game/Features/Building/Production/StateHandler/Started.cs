using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Production;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public unsafe class Started : IProductionStateProvider
    {
        public void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;

            aspect.ProductionProcessData.ValueRW.CycleInProgress = 1;

            methods->StartFullLoadTimer(aspect, ecb);
            methods->StartOneLoadTimer(aspect, ecb);

            aspect.AddEvent(BuildingEvent.Production_Timers_Started);

            methods->TakeProductsForOneLoad(aspect);
            methods->UpdateInProductionBoxUI(aspect);

            aspect.SetCurrentCycle(aspect.ProductionProcessData.ValueRO.CycleInProgress);

            methods->SetNewState(ProductionState.InProgress, aspect);
        }
    }
}