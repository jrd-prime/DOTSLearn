using Jrd.Gameplay.Storage;
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
        /// Update item quantity if panel opened and quantity changed
        /// </summary>
        public void UpdateItemQuantity(object item, int value);
    }
}