using Unity.Entities;

namespace Jrd.GameStates.BuildingState.BuildingPanel
{
    public struct BlueprintsShopPanelData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}