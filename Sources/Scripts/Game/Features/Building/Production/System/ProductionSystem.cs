using Sources.Scripts.CommonData;
using Sources.Scripts.CommonData.Building;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Production.System
{
    [UpdateInGroup(typeof(JInitSimulationSystemGroup))]
    public partial class ProductionSystem : SystemBase
    {
        private BuildingDataAspect _buildingData;
        private int _cycleInProgress;

        private Production _production;

        protected override void OnCreate()
        {
            RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
            _production = new Production();
        }

        protected override void OnUpdate()
        {
            foreach (var aspect in SystemAPI.Query<BuildingDataAspect>())
            {
                EntityCommandBuffer ecb = SystemAPI
                    .GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
                    .CreateCommandBuffer(World.Unmanaged);


                _production.Process(
                    aspect.BuildingData.ProductionState,
                    aspect,
                    ecb);

                // productionState = ProductionState.EnoughProducts; // test
            }
        }
    }
}