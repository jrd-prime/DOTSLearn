using Unity.Entities;

namespace Sources.Scripts.CommonData.Building
{
    public struct AddEventToBuildingData : IComponentData
    {
        public BuildingEvent Value;
    }
}