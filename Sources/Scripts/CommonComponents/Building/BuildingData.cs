using Sources.Scripts.CommonComponents.Production;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Sources.Scripts.CommonComponents.Building
{
    public struct BuildingData : IComponentData
    {
        public Entity Self;
        public FixedString64Bytes Name;
        public Entity Prefab;
        public FixedString64Bytes Guid;
        public float3 WorldPosition;
        public BuildingNameId NameId;
        public NativeQueue<BuildingEvent> BuildingEvents;

        // TODO remove, buff
        public int Level;
        public float ItemsPerHour;
        public int LoadCapacity;
        public int MaxStorage;
        public ProductionState ProductionState;
    }
}