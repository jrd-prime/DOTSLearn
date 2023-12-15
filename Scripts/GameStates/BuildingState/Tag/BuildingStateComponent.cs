using Jrd.Build.old;
using Unity.Collections;
using Unity.Entities;

namespace Jrd.GameStates.BuildingState.Tag
{
    public struct BuildingStateComponent : IComponentData
    {
        public Entity SelectedPrefab;
        public int SelectedPrefabID;
        public int PrefabsCount;
        public NativeArray<PrefabBufferElements> PrefabsBufferElementsCache; // TODO подумать
        public Entity TempEntity;
    }
}