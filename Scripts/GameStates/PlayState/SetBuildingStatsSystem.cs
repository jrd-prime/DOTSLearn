using Jrd.GameplayBuildings;
using Unity.Entities;

namespace Jrd.GameStates.PlayState
{
    public partial struct SetBuildingStatsSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (buildingData, entity) in SystemAPI
                         .Query<BuildingData>()
                         .WithAll<SelectedBuildingTag>()
                         .WithEntityAccess())
            {
            }
        }
    }
}