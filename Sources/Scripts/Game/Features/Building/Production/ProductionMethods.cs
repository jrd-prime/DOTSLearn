using Sources.Scripts.CommonComponents.Building;
using Sources.Scripts.CommonComponents.Production;
using Sources.Scripts.CommonComponents.Storage;
using Sources.Scripts.CommonComponents.Storage.Data;
using Sources.Scripts.Timer;
using Unity.Entities;
using BuildingDataAspect = Sources.Scripts.CommonComponents.Building.BuildingDataAspect;

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

        public void UpdateProductionUI(BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.InProductionBoxDataUpdated);

        public void UpdateManufacturedUI(BuildingDataAspect aspect) =>
            aspect.AddEvent(BuildingEvent.ManufacturedBoxDataUpdated);

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