using Jrd.GameplayBuildings;
using Jrd.PlayState;
using Unity.Entities;

namespace Jrd.GameStates.PlayState
{
    public partial struct BuildingConfigSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            var a = BuildingConfigPanelMono.Instance;
            foreach (var (buildingData, entity) in SystemAPI
                         .Query<BuildingData>()
                         .WithAll<SelectedBuildingTag>()
                         .WithEntityAccess())
            {
                a.SetLevel(buildingData.Level);
                a.SetSpeed(buildingData.ItemsPerHour);
                a.SetLoadCapacity(buildingData.LoadCapacity);
                a.SetMaxStorage(buildingData.MaxStorage);
            }
        }
    }
}