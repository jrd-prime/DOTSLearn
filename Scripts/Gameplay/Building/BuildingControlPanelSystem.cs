using Jrd.GameStates;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.GameStates.PlayState;
using Jrd.UI.BuildingControlPanel;
using Unity.Entities;

namespace Jrd.Gameplay.Building
{
    public partial struct BuildingControlPanelSystem : ISystem
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


            var a = BuildingControlPanelUI.Instance;
            foreach (var (buildingData, entity) in SystemAPI
                         .Query<BuildingData>()
                         .WithAll<InitializeTag, SelectedBuildingTag>()
                         .WithEntityAccess())
            {
                var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
                var requiredItemsBuffer = SystemAPI.GetBuffer<BuildingRequiredItemsBuffer>(bufferEntity);
                var manufacturedItemsBuffer = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

                a.SetLevel(buildingData.Level);
                a.SetProductivity(buildingData.ItemsPerHour);
                a.SetLoadCapacity(buildingData.LoadCapacity);
                a.SetStorageCapacity(buildingData.MaxStorage);

                // TODO think
                a.SetSpecName(Spec.Productivity, manufacturedItemsBuffer.ElementAt(0)._item.ToString());
                a.SetSpecName(Spec.LoadCapacity, requiredItemsBuffer.ElementAt(0)._item.ToString());
                a.SetSpecName(Spec.StorageCapacity, manufacturedItemsBuffer.ElementAt(0)._item.ToString());

                a.SetLineInfo(requiredItemsBuffer, manufacturedItemsBuffer);

                ecb.RemoveComponent<InitializeTag>(entity);
            }
        }
    }
}