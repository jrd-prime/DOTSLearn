using Unity.Collections;
using Unity.Entities;

namespace Jrd
{
    public struct GameBuildingsData : IComponentData
    {
        public NativeHashMap<FixedString64Bytes, BuildingData> GameBuildings;
    }
}