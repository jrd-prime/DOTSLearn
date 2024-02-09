using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class RequiredBoxUI : ProductionBox
    {
        public RequiredBoxUI(VisualElement panel, VisualTreeAsset boxItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.RequiredBoxContainerId);
            NameLabel = Container.Q<Label>(BCPNamesID.RequiredBoxNameLabelId);
            ItemContainerTemplate = boxItemTemplate;
        }
    }
}