using Unity.Collections;
using Unity.Entities;

namespace Jrd.Utils._Deb
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