using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Production;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class Finished : IProductionStateHandler
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;

            methods->PutProductsFromOneLoad(aspect);
            methods->UpdateManufacturedUI(aspect);

            methods->SetNewState(ProductionState.NotEnoughProducts, aspect);

            // обновить все юаи продакшена 
            // показать/уведомить что задание завершено
        }
    }
}