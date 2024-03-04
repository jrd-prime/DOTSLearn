using Unity.Entities;

namespace Sources.Scripts.CommonComponents
{
    public struct BuildingStateData : IComponentData
    {
        public bool IsInitialized;
        public int BuildingPrefabsCount;
    }
}