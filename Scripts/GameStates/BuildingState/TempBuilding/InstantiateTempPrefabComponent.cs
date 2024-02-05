using Jrd.Gameplay.Building;
using Jrd.Gameplay.Building.ControlPanel;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState.TempBuilding
{
    public struct InstantiateTempPrefabComponent : IComponentData
    {
        public BuildingData BuildingData;
    }
}