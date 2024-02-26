using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace CommonComponents.Building
{
    public struct BlueprintsBuffer : IBufferElementData
    {
        public Entity Self;

        public BuildingCategoryId CategoryId;
        public BuildingNameId NameId;
        public FixedString64Bytes Name;
        public float2 Size;

        public int Level;
        public float ItemsPerHour;
        public int LoadCapacity;
        public int StorageCapacity;
    }
}