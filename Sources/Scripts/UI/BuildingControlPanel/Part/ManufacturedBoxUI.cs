using UnityEngine.UIElements;

namespace Sources.Scripts.UI.BuildingControlPanel.Part
{
    public class ManufacturedBoxUI: ProductionBox
    {
        public ManufacturedBoxUI(VisualElement panel, VisualTreeAsset boxItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.ManufacturedBoxContainerId);
            NameLabel = panel.Q<Label>(BCPNamesID.ManufacturedBoxNameLabelId);
            ItemContainerTemplate = boxItemTemplate;

        }
    }
}