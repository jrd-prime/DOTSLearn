using UnityEngine.UIElements;

namespace GamePlay.UI.BuildingControlPanel.Part
{
    public class MainStorageUI : StorageUI
    {
        public MainStorageUI(VisualElement panel, VisualTreeAsset mainStorageItemTemplate)
        {
            Container = panel.Q<VisualElement>(BCPNamesID.MainStorageContainerId);
            NameLabel = Container.Q<Label>(BCPNamesID.MainStorageNameLabelId);
            ItemContainerTemplate = mainStorageItemTemplate;
        }
    }
}