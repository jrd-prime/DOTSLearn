using Jrd.Gameplay.Products;
using Unity.Collections;

namespace Jrd.UI.BuildingControlPanel
{
    public interface IBuildingControlPanelStorage
    {
        /// <summary>
        /// Set items when building info panel opened
        /// </summary>
        public void SetItems(NativeList<ProductData> list);

        /// <summary>
        /// Set empty UI for storage if items list empty
        /// </summary>
        public void SetEmptyStorage();

        /// <summary>
        /// Update item quantity if panel opened and quantity changed
        /// </summary>
        public void UpdateItemQuantity(object item, int value);
    }
}