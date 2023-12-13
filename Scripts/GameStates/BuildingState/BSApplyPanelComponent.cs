using Unity.Entities;

namespace Jrd.Build.EditModePanel
{
    public struct BSApplyPanelComponent : IComponentData
    {
        public bool ShowPanel;
        public bool IsVisible;
    }
}