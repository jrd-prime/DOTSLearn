using Unity.Entities;

namespace Jrd.Gameplay.Building
{
    public struct AddEventToBuildingData : IComponentData
    {
        public BuildingEvent Value;
    }
}