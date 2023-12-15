using Unity.Collections;
using Unity.Entities;

namespace Jrd.Build.old
{
    public struct BuildPrefabComponent : IComponentData
    {
        public Entity BuildPrefab;
    }
    
    public struct PrefabBufferElements : IBufferElementData
    {
        public Entity PrefabEntity;
        public FixedString32Bytes PrefabName;
    }
}