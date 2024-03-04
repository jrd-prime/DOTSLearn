﻿
using Unity.Entities;
using UnityEngine;
using BuildingDataAspect = Sources.Scripts.CommonComponents.Building.BuildingDataAspect;

namespace Sources.Scripts.Game.Features.Building.Production.StateHandler
{
    public class Stopped : IProductionStateHandler
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