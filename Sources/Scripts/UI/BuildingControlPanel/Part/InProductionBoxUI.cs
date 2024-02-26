using UnityEngine.UIElements;

namespace Sources.Scripts.UI.BuildingControlPanel.Part
{
    public class InProductionBoxUI : ProductionBox
    {
        public InProductionBoxUI(VisualElement panel, VisualTreeAsset boxItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.RequiredBoxContainerId);
            NameLabel = panel.Q<Label>(BCPNamesID.RequiredBoxNameLabelId);
            ItemContainerTemplate = boxItemTemplate;
        }
    }
}