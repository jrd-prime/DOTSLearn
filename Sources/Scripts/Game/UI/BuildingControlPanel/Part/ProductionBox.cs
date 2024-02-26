using System;
using CommonComponents.Product;
using GamePlay.Common.Constants;
using GamePlay.Features.Building.Products;
using GamePlay.Features.Building.Products.Component;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.UI.BuildingControlPanel.Part
{
    public abstract class ProductionBox : IProductsItemsContainer
    {
        protected VisualElement Container;
        protected Label NameLabel;
        protected VisualTreeAsset ItemContainerTemplate;

        protected string ContainerId;
        protected string NameLabelId;

        public void SetItems(NativeList<ProductData> productsData)
        {
            Container.Clear();

            if (productsData.IsEmpty || productsData.Length == 0)
            {
                SetEmptyContainerItems();
            }

            for (int i = 0; i < productsData.Length; i++)
            {
                var item = GetFilledItem(productsData[i].Name, productsData[i].Quantity);

                Container.Add(item);

                // var an = item
                //     .Q<VisualElement>("prod-line-item-icon-cont")?
                //     .experimental
                //     .animation
                //     .Scale(1.2f, 100);
                // if (an != null)
                // {
                //     an.onAnimationCompleted = () => item
                //         .Q<VisualElement>("prod-line-item-icon-cont")
                //         .experimental
                //         .animation
                //         .Scale(1f, 100);
                // }
            }
        }

        private VisualElement GetFilledItem(Product item, int itemCount)
        {
            // TODO all method / refact
            VisualElement template = ItemContainerTemplate.Instantiate();

            var itemContainer = template.Q<VisualElement>("ico1");
            var iteml = template.Q<Label>("item-count-label");

            // Icon //TODO getpath, enum to string?lol?
            var iconPath = Names.GoodsIconsPath + item.ToString().ToLower();
            
            var iconSprite = Utility.Utils.LoadFromResources<Sprite>(iconPath, this);

            itemContainer.style.backgroundImage = new StyleBackground(iconSprite);
            // NameLabel.text = itemCount.ToString();


            iteml.text = itemCount.ToString();


            return template;
        }

        public void SetEmptyContainerItems()
        {
            throw new NotImplementedException();
        }

        public void UpdateItemQuantity(object item, int value)
        {
            throw new NotImplementedException();
        }
    }
}