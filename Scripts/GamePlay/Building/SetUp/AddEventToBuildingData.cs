using Unity.Entities;

namespace GamePlay.Building.SetUp
{
    public struct AddEventToBuildingData : IComponentData
    {
        public BuildingEvent Value;
    }
}