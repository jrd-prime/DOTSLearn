using GamePlay.Building.ControlPanel.Component;
using Unity.Entities;

namespace GamePlay.Building.TempBuilding.Component
{
    public struct InstantiateTempBuildingData : IComponentData
    {
        public BuildingData BuildingData;
    }
}