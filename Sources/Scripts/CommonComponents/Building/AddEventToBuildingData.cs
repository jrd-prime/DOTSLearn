using Unity.Entities;

namespace Sources.Scripts.CommonComponents.Building
{
    public struct AddEventToBuildingData : IComponentData
    {
        public BuildingEvent Value;
    }
}