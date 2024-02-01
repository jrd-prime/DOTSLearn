using Jrd.GameplayBuildings;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.PlayState;
using Unity.Entities;
using UnityEngine;

namespace Jrd.GameStates.PlayState
{
    public partial struct BuildingConfigSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<JBuildingsPrefabsTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var a = BuildingConfigPanelMono.Instance;
            foreach (var (buildingData, entity) in SystemAPI
                         .Query<BuildingData>()
                         .WithAll<SelectedBuildingTag>()
                         .WithEntityAccess())
            {
                var bufferEntity = SystemAPI.GetSingletonEntity<JBuildingsPrefabsTag>();
                var buffer = SystemAPI.GetBuffer<BuildingManufacturedItemsBuffer>(bufferEntity);

                a.SetLevel(buildingData.Level);
                a.SetSpeed(buildingData.ItemsPerHour);
                a.SetLoadCapacity(buildingData.LoadCapacity);
                a.SetMaxStorage(buildingData.MaxStorage);
                
                // TODO think
                a.SetStatNames(buffer.ElementAt(0)._manufacturedItem.ToString());
            }
        }
    }
}