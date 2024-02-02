using Jrd.GameplayBuildings;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.PlayState;
using Unity.Entities;

namespace Jrd.GameStates.PlayState
{
    public partial struct BuildingConfigSystem : ISystem
    {
        private BeginSimulationEntityCommandBufferSystem.Singleton sys;
        private EntityCommandBuffer ecb;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
            state.RequireForUpdate<JBuildingsPrefabsTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            sys = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            ecb = sys.CreateCommandBuffer(state.WorldUnmanaged);


            var a = BuildingConfigPanelMono.Instance;
            foreach (var (buildingData, entity) in SystemAPI
                         .Query<BuildingData>()
                         .WithAll<InitializeTag, SelectedBuildingTag>()
                         .WithEntityAccess())
            {
                var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
                var requiredItemsBuffer = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
                var manufacturedItemsBuffer = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

                a.SetLevel(buildingData.Level);
                a.SetSpeed(buildingData.ItemsPerHour);
                a.SetLoadCapacity(buildingData.LoadCapacity);
                a.SetMaxStorage(buildingData.MaxStorage);

                // TODO think
                a.SetStatNames(manufacturedItemsBuffer.ElementAt(0)._manufacturedItem.ToString());

                a.SetProductionLineInfo(requiredItemsBuffer, manufacturedItemsBuffer);

                ecb.RemoveComponent<InitializeTag>(entity);
            }
        }
    }
}