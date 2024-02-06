using Jrd.Gameplay.Product;
using Jrd.Gameplay.Storage;
using Jrd.Utils.Const;
using Unity.Collections;
using UnityEngine;
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
            if (list.IsEmpty)
            {
                Debug.Log("Empty storage list");
                SetEmptyStorage();
            }

            Container.Clear();

            for (int i = 0; i < list.Length; i++)
            {
                var item = GetFilledItem(list[i].Name, list[i].Quantity);

                Container.Add(item);
            }
        }

        public void SetEmptyStorage()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateItemQuantity(object item, int value)
        {
            throw new System.NotImplementedException();
        }

        //TODO repeated
        private VisualElement GetFilledItem(Product item, int itemCount)
        {
            // TODO all method / refact

            VisualElement template = ItemContainerTemplate.Instantiate();

            var itemContainer = template.Q<VisualElement>(BCPNamesID.ProdLineItemContainerId);
            itemContainer.style.height = 52;
            itemContainer.style.width = 52;
            var itemCountLabel = template.Q<Label>(BCPNamesID.ProdLineItemCountLabelId);

            // Icon //TODO getpath, enum to string?lol?
            var iconPath = GameConst.GoodsIconsPath + item.ToString().ToLower();
            var iconSprite = Utils.Utils.LoadFromResources<Sprite>(iconPath, this);

            itemContainer.style.backgroundImage = new StyleBackground(iconSprite);
            itemCountLabel.text = itemCount.ToString();

            return template;
        }
    }
}