using Unity.Entities;
using UnityEngine;
using BuildingDataAspect = Sources.Scripts.CommonData.Building.BuildingDataAspect;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class NotEnough : IProductionStateProvider
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;
            // // показывать в панели билдинга что нету продуктов для производства в инпродакшн боксе
        }
    }
}