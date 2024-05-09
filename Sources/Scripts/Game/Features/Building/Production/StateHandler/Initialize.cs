using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Production;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class Initialize : IProductionStateProvider
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            // BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;
            // показывать в панели билдинга что нету продуктов для производства в инпродакшн боксе
            
            data.Aspect.SetProductionState(ProductionState.NotEnoughProducts);
        }
    }
}