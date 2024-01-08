using Unity.Collections;
using Unity.Entities;

namespace Jrd
{
    public struct BuildingData : IComponentData
    {
        public FixedString64Bytes Name;
        public Entity Prefab;

        
    }
}