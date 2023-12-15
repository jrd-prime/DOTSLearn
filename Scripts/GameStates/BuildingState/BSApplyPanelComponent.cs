using Unity.Entities;

namespace Jrd.GameStates.BuildingState
{
    public struct BSApplyPanelComponent : IComponentData
    {
        public bool ShowPanel;
        public bool IsVisible;
    }
}