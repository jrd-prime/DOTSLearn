using GamePlay.Features.Building.ControlPanel.Component;
using Unity.Entities;

namespace GamePlay.Features.Building.PlaceBuilding.Component
{
    public struct InstantiateTempBuildingData : IComponentData
    {
        public BuildingData BuildingData;
    }
}