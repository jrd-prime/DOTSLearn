using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    public struct BuildingStateData : IComponentData
    {
        public bool IsInitialized;
        public int BuildingPrefabsCount;
    }
}