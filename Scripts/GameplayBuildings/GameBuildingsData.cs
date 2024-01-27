using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameplayBuildings
{
    public struct GameBuildingsData : IComponentData
    {
        public NativeHashMap<FixedString64Bytes, BuildingData> GameBuildings;
    }
}