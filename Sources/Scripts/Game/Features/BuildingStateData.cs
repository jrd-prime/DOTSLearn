using Unity.Entities;

namespace Sources.Scripts.Game.Features
{
    public struct BuildingStateData : IComponentData
    {
        public bool IsInitialized;
        public int BuildingPrefabsCount;
    }
}