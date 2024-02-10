using Jrd.Gameplay.Products;
using Jrd.MyUtils;
using Jrd.MyUtils.Const;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    public abstract class StorageUI : IProductsItemsContainer
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
                SetEmptyContainerItems();
            }

            Container.Clear();

            for (int i = 0; i < list.Length; i++)
            {
                var item = GetFilledItem(list[i].Name, list[i].Quantity);


                Container.Add(item);

                var an = item
                    .Q<VisualElement>("prod-line-item-icon-cont")?
                    .experimental
                    .animation
                    .Scale(1.2f, 100);
                if (an != null)
                {
                    an.onAnimationCompleted = () => item
                        .Q<VisualElement>("prod-line-item-icon-cont")
                        .experimental
                        .animation
                        .Scale(1f, 100);
                }
            }
        }

        public void SetEmptyContainerItems()
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
            itemContainer.style.height = 50;
            itemContainer.style.width = 50;
            var itemCountLabel = template.Q<Label>(BCPNamesID.ProdLineItemCountLabelId);

            // Icon //TODO getpath, enum to string?lol?
            var iconPath = GameConst.GoodsIconsPath + item.ToString().ToLower();
            var iconSprite = Utils.LoadFromResources<Sprite>(iconPath, this);

            itemContainer.style.backgroundImage = new StyleBackground(iconSprite);
            itemCountLabel.text = itemCount.ToString();

            return template;
        }
    }
}