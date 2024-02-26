using Unity.Entities;

namespace GamePlay.Features.Building.Events
{
    public struct AddEventToBuildingData : IComponentData
    {
        public BuildingEvent Value;
    }
}