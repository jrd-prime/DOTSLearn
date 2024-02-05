using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelMainStorageUI : BuildingControlPanelStorage
    {
        public BuildingControlPanelMainStorageUI(VisualElement panel, VisualTreeAsset mainStorageItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.MainStorageContainerId);
            NameLabel = Container.Q<Label>(BCPNamesID.MainStorageNameLabelId);
            ItemContainerTemplate = mainStorageItemTemplate;
        }
    }
}