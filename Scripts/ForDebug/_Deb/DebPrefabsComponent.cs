using Unity.Collections;
using Unity.Entities;

namespace Jrd.ForDebug._Deb
{
    public struct DebPrefabsComponent : IComponentData
    {
        public Entity prefab;
    }

    public struct DebPrefabBufferElements : IBufferElementData
    {
        public Entity PrefabEntity;
        public FixedString64Bytes PrefabName;
    }
}