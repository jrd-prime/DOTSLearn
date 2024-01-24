using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState.Prefabs
{
    public struct BuildPrefabsComponent : IComponentData
    {
        public Entity BuildPrefab;
    }

    public struct BuildingsPrefabsBuffer : IBufferElementData
    {
        public Entity PrefabEntity;
        public FixedString64Bytes PrefabName;
    }
}