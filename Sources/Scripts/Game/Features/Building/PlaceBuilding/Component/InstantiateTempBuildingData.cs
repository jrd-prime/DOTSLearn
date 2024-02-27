using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding.Component
{
    public struct InstantiateTempBuildingData : IComponentData
    {
        public BuildingData BuildingData;
    }
}