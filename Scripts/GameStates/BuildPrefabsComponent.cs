using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates
{
    public struct BuildPrefabsComponent : IComponentData
    {
        public Entity BuildPrefab;
    }

    public struct PrefabBufferElements : IBufferElementData
    {
        public Entity PrefabEntity;
        public FixedString32Bytes PrefabName;
    }
}