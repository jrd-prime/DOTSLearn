﻿using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Production;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public interface IProductionMethods
    {
        void SetNewState(ProductionState value, BuildingDataAspect aspect);
        void TakeProductsForOneLoad(BuildingDataAspect aspect);
        void PutProductsFromOneLoad(BuildingDataAspect aspect);

        void UpdateInProductionBoxUI(BuildingDataAspect aspect);
        void UpdateManufacturedBoxUI(BuildingDataAspect aspect);
        void UpdateProductionTimersUI(BuildingDataAspect aspect);

        void StartOneLoadTimer(BuildingDataAspect aspect, EntityCommandBuffer ecb);
        void StartFullLoadTimer(BuildingDataAspect aspect, EntityCommandBuffer ecb);
    }
}