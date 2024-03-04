using Sources.Scripts.CommonComponents.Production;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production
{
    public interface IProductionMethods
    {
        void SetNewState(ProductionState value, CommonComponents.test.BuildingDataAspect aspect);
        void TakeProductsForOneLoad(CommonComponents.test.BuildingDataAspect aspect);
        void PutProductsFromOneLoad(CommonComponents.test.BuildingDataAspect aspect);

        void UpdateProductionUI(CommonComponents.test.BuildingDataAspect aspect);
        void UpdateManufacturedUI(CommonComponents.test.BuildingDataAspect aspect);

        void StartOneLoadTimer(CommonComponents.test.BuildingDataAspect aspect, EntityCommandBuffer ecb);
        void StartFullLoadTimer(CommonComponents.test.BuildingDataAspect aspect, EntityCommandBuffer ecb);
    }
}