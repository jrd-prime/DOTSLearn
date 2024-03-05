using Sources.Scripts.CommonData.Building;
using Sources.Scripts.CommonData.Production;
using Sources.Scripts.CommonData.Storage.Data;
using Sources.Scripts.Timer;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public struct ProductionMethods : IProductionMethods
    {
        #region Building data

        public void SetNewState(ProductionState value, BuildingDataAspect aspect) =>
            aspect.SetProductionState(value);

        public void TakeProductsForOneLoad(BuildingDataAspect aspect) =>
            aspect.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Reduce,
                ProductsData = aspect.RequiredProductsData.Value
            });

        public void PutProductsFromOneLoad(BuildingDataAspect aspect) =>
            aspect.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Manufactured,
                ChangeType = ChangeType.Increase,
                ProductsData = aspect.ManufacturedProductsData.Value
            });

        #endregion

        #region Set events

        public void UpdateInProductionBoxUI(BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.InProductionBox_Updated);

        public void UpdateManufacturedBoxUI(BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.ManufacturedBox_Updated);

        public void UpdateProductionTimersUI(BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.Production_Timers_InProgressUpdate);

        #endregion

        #region Timers

        public void StartOneLoadTimer(BuildingDataAspect aspect, EntityCommandBuffer ecb)
        {
            new JTimer().StartNewTimer(
                aspect.Self,
                TimerType.OneLoadCycle,
                aspect.GetOneProductManufacturingTime(),
                ecb);
        }

        public void StartFullLoadTimer(BuildingDataAspect aspect, EntityCommandBuffer ecb)
        {
            new JTimer().StartNewTimer(
                aspect.Self,
                TimerType.FullLoadCycle,
                aspect.GetLoadedProductsManufacturingTime(),
                ecb);
        }

        #endregion
    }
}