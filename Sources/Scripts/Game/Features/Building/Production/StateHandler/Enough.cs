using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.CommonComponents.Storage.Service;
using Unity.Entities;
using UnityEngine;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class Enough : IProductionStateHandler
    {
        public unsafe void Run(ProductionProcessDataWrapper data)
        {
            Debug.Log($"{GetType().Name} {data.Aspect.BuildingData.Name}");

            BuildingDataAspect aspect = data.Aspect;
            EntityCommandBuffer ecb = data.Ecb;
            ProductionMethods* methods = data.ProductionMethodsPtr;

            // 0 проверить есть ли доступное место в боксе произведенных для старта нового цикла
            // если нет уведомить/показать что надо освободить место в боксе произведенных
            if (!ManufacturedService.IsEnoughSpaceInManufacturedBox())
            {
                Debug.LogWarning("Not enough space in manufactured box!");
            }

            // 1 рассчитать продукты
            // var loadedProducts = _buildingData.ProductionProcessData.PreparedProducts;

            // 2 понять за сколько раз переработаем
            // var maxLoads = _buildingData.ProductionProcessData.MaxLoads;

            // 3 какие тайминги
            Debug.Log("oneProductTime " + aspect.GetOneProductManufacturingTime() + " sec");
            Debug.Log("loadedTime " + aspect.GetLoadedProductsManufacturingTime() + " sec");

            // Set timers
            aspect.SetFullLoadedProductsTimer();
            aspect.SetOneProductTimer();

            aspect.SetProductionState(ProductionState.Started);
        }
    }
}