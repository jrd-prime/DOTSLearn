using Unity.Entities;

namespace Sources.Scripts.Game.GameStates.BuildingState
{
    public struct BuildingStateData : IComponentData
    {
        public bool IsInitialized;
        public int BuildingPrefabsCount;
    }
}