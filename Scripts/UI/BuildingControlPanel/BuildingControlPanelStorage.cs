using Jrd.Gameplay.Storage;
using Unity.Collections;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public abstract class BuildingControlPanelStorage : IBuildingControlPanelStorage
    {
        protected VisualElement Container;
        protected Label NameLabel;
        protected VisualTreeAsset ItemContainerTemplate;

        protected string ContainerId;
        protected string NameLabelId;

        public void SetItems(NativeList<ProductData> list)
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItemQuantity(object item, int value)
        {
            throw new System.NotImplementedException();
        }
    }
}