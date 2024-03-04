using Sources.Scripts.CommonComponents.Production;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class InProgress : IProductionStateHandler
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            CommonComponents.test.BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;

            var cycleInProgress = aspect.ProductionProcessData.ValueRO.CycleInProgress;
            var currentCycle = aspect.ProductionProcessData.ValueRO.CurrentCycle;
            var lastCycleEnd = aspect.ProductionProcessData.ValueRO.LastCycleEnd;

            var maxLoads = aspect.ProductionProcessData.ValueRO.MaxLoads;

            Debug.LogWarning("last cycle end " + lastCycleEnd);
            if (lastCycleEnd) methods->SetNewState(ProductionState.Finished, aspect);

            Debug.LogWarning("curr cycle " + cycleInProgress);

            if (cycleInProgress != currentCycle)
            {
                methods->PutProductsFromOneLoad(aspect);
                methods->UpdateManufacturedUI(aspect);

                if (cycleInProgress != maxLoads)
                {
                    Debug.Log("new timer");
                    methods->TakeProductsForOneLoad(aspect);
                    methods->UpdateProductionUI(aspect);
                    methods->StartOneLoadTimer(aspect, ecb);
                    aspect.ProductionProcessData.ValueRW.CycleInProgress = currentCycle;
                }
                else
                {
                    Debug.Log("temp = max load");
                }
            }

            // сетить и обновлять таймеры
            // когда заканчивается один цикл - обновить юай произмеденных
            // - взять еще продукты из продакшн бокса для наового цикла и обновить юай

            // проверить есть ли мето в произведенных для нвого цикла

            // если нету места для нового цикла, то стэйт в стоппед и показать кведомление над зданием
            // СОХРАНИТЬ ТАЙМЕРЫ И КОЛВО ПРОДУКТОВ ЕСЛИ ПЕРЕХОДИМ В СТОП ИЗ-ЗА ПОЛНОГО СКЛАДА
        }
    }
}