using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class ManufacturedBoxUI: ProductionBox
    {
        public ManufacturedBoxUI(VisualElement panel, VisualTreeAsset boxItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.ManufacturedBoxContainerId);
            NameLabel = Container.Q<Label>(BCPNamesID.ManufacturedBoxNameLabelId);
            ItemContainerTemplate = boxItemTemplate;

        }
    }
}