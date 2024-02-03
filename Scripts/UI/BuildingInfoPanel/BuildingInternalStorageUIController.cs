using UnityEngine.UIElements;

namespace Jrd.UI.BuildingInfoPanel
{
    public class BuildingInternalStorageUIController : IBuildingInternalStorage
    {
        private readonly VisualElement _container;
        private Label _nameLabel;

        private const string ContainerId = "internal-storage-cont";
        private const string ItemContainerId = "prod-line-item-cont";
        private const string ItemCountLabelId = "prod-line-item-count";
        private const string NameLabelId = "internal-storage-name-label";

        private readonly VisualTreeAsset _itemContainerTemplate;
        private readonly VisualTreeAsset _arrowTemplate;

        public BuildingInternalStorageUIController(VisualElement panel)
        {
            _container = panel.Q<VisualElement>(ContainerId);
            _nameLabel = _container.Q<Label>(NameLabelId);
        }

        public void SetItems(object list)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItemQuantity(object item, int value)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IBuildingInternalStorage
    {
        /// <summary>
        /// Set items when building info panel opened
        /// </summary>
        public void SetItems(object list);

        /// <summary>
        /// Update item quantity if panel opened and quantity changed
        /// </summary>
        public void UpdateItemQuantity(object item, int value);
    }
}