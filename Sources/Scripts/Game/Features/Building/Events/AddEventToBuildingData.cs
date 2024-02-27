using Sources.Scripts.CommonComponents.Building;
using Unity.Entities;

namespace Sources.Scripts.Game.Features.Building.Events
{
    public struct AddEventToBuildingData : IComponentData
    {
        public BuildingEvent Value;
    }
}