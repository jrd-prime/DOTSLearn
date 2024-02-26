using GamePlay.Features.Building.Events;
using GamePlay.Features.Building.Production;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace GamePlay.Features.Building.ControlPanel.Component
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