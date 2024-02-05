using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelWarehouseUI : BuildingControlPanelStorage
    {
        public BuildingControlPanelWarehouseUI(VisualElement panel, VisualTreeAsset warehouseItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.WarehouseContainerId);
            NameLabel = Container.Q<Label>(BCPNamesID.WarehouseNameLabelId);
            ItemContainerTemplate = warehouseItemTemplate;
        }
    }
}