using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.CommonComponents.test;
using Sources.Scripts.Timer;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public struct ProductionMethods : IProductionMethods
    {
        #region Building data

        public void SetNewState(ProductionState value, CommonComponents.test.BuildingDataAspect aspect) =>
            aspect.SetProductionState(value);

        public void TakeProductsForOneLoad(CommonComponents.test.BuildingDataAspect aspect) =>
            aspect.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.InProduction,
                ChangeType = ChangeType.Reduce,
                ProductsData = aspect.RequiredProductsData.Value
            });

        public void PutProductsFromOneLoad(CommonComponents.test.BuildingDataAspect aspect) =>
            aspect.ChangeProductsQuantity(new ChangeProductsQuantityData
            {
                StorageType = StorageType.Manufactured,
                ChangeType = ChangeType.Increase,
                ProductsData = aspect.ManufacturedProductsData.Value
            });

        #endregion

        #region Set events

        public void UpdateProductionUI(CommonComponents.test.BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.InProductionBoxDataUpdated);

        public void UpdateManufacturedUI(CommonComponents.test.BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.ManufacturedBoxDataUpdated);

        #endregion

        #region Timers

        public void StartOneLoadTimer(CommonComponents.test.BuildingDataAspect aspect, EntityCommandBuffer ecb)
        {
            new JTimer().StartNewTimer(
                aspect.Self,
                TimerType.OneLoadCycle,
                aspect.GetOneProductManufacturingTime(),
                ecb);
        }

        public void StartFullLoadTimer(CommonComponents.test.BuildingDataAspect aspect, EntityCommandBuffer ecb)
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