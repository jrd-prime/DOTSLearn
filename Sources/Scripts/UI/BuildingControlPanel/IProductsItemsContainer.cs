using Sources.Scripts.CommonData.Product;
using Unity.Collections;

namespace Sources.Scripts.UI.BuildingControlPanel
{
    /// <summary>
    /// Contains methods for UI with container for list products items
    /// </summary>
    public interface IProductsItemsContainer
    {
        /// <summary>
        /// Set items in UI building control panel
        /// </summary>
        public void SetItems(NativeList<ProductData> productsData);

        /// <summary>
        /// Set empty UI for storage if items list empty
        /// </summary>
        public void SetEmptyContainerItems();

        /// <summary>
        /// Update item quantity if panel opened and quantity changed
        /// </summary>
        public void UpdateItemQuantity(object item, int value);
    }
}