using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Production;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class Finished : IProductionStateProvider
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;

            methods->PutProductsFromOneLoad(aspect);
            methods->UpdateManufacturedBoxUI(aspect);

            methods->SetNewState(ProductionState.NotEnoughProducts, aspect);

            // обновить все юаи продакшена 
            // показать/уведомить что задание завершено
        }
    }
}