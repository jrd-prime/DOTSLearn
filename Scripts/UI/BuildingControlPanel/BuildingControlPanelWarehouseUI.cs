using Jrd.Gameplay.Storage;
using Unity.Collections;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelWarehouseUI : IBuildingWarehouse
    {
        private readonly VisualElement _container;
        private Label _nameLabel;

        private VisualTreeAsset _itemContainerTemplate;

        private const string ContainerId = "internal-storage-cont";
        private const string NameLabelId = "internal-storage-name-label";

        private const string ItemContainerId = "prod-line-item-cont";
        private const string ItemCountLabelId = "prod-line-item-count";

        public BuildingControlPanelWarehouseUI(VisualElement panel, VisualTreeAsset internalStorageItemTemplate)
        {
            _container = panel.Q<VisualElement>(ContainerId);
            _nameLabel = _container.Q<Label>(NameLabelId);
            _itemContainerTemplate = internalStorageItemTemplate;
        }

        public void SetItems(NativeList<ProductData> internalWarehouseProducts)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItemQuantity(object item, int value)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IBuildingWarehouse
    {
        /// <summary>
        /// Set items when building info panel opened
        /// </summary>
        public void SetItems(NativeList<ProductData> list);

        /// <summary>
        /// Update item quantity if panel opened and quantity changed
        /// </summary>
        public void UpdateItemQuantity(object item, int value);
    }
}