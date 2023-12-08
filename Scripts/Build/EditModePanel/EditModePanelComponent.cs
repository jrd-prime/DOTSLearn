using Unity.Entities;

namespace Jrd.Build.EditModePanel
{
    public struct EditModePanelComponent : IComponentData
    {
        public bool ShowPanel;
        public bool IsVisible;
    }
}