using Sources.Scripts.CommonData.Building;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class Stopped : IProductionStateProvider
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            // ожидание когда освободится место на складе и как-то продолжить цикл
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;
        }
    }
}