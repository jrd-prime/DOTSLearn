using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.PlaceBuilding
{
    public struct InstantiateTempBuildingData : IComponentData
    {
        public BuildingData BuildingData;
    }
}