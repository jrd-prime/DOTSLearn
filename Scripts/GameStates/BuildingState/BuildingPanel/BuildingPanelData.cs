using Unity.Entities;

namespace Jrd.GameStates.BuildingState.BuildingPanel
{
    public struct BuildingPanelData : IComponentData
    {
        public bool SetVisible;
        public int BuildingPrefabsCount;
    }
}