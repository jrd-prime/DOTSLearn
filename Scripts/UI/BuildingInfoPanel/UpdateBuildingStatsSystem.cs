using Jrd.GameplayBuildings;
using Jrd.GameStates.PlayState;
using Unity.Entities;

namespace Jrd.UI.BuildingInfoPanel
{
    /// <summary>
    /// Update building stats on update tag exist
    /// </summary>
    public partial struct UpdateBuildingStatsSystem : ISystem
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