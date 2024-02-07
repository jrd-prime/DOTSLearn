using Jrd.Gameplay.Building.ControlPanel;
using Unity.Entities;

namespace Jrd.Gameplay.Building.TempBuilding
{
    public struct InstantiateTempPrefabComponent : IComponentData
    {
        public BuildingData BuildingData;
    }
}