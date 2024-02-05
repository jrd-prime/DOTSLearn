using Unity.Entities;

namespace Jrd.GameStates.BuildingState.BuildingPanel
{
    public struct BlueprintsShopData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}