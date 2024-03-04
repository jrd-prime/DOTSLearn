using Unity.Entities;

namespace Sources.Scripts.CommonData
{
    public struct BuildingStateData : IComponentData
    {
        public bool IsInitialized;
        public int BuildingPrefabsCount;
    }
}