using Unity.Entities;

namespace GamePlay.GameStates.BuildingState
{
    public struct BuildingStateData : IComponentData
    {
        public bool IsInitialized;
        public int BuildingPrefabsCount;
    }
}