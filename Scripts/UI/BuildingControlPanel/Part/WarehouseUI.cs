using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public class WarehouseUI : Storage
    {
        public WarehouseUI(VisualElement panel, VisualTreeAsset warehouseItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.WarehouseContainerId);
            NameLabel = Container.Q<Label>(BCPNamesID.WarehouseNameLabelId);
            ItemContainerTemplate = warehouseItemTemplate;
        }
    }
}