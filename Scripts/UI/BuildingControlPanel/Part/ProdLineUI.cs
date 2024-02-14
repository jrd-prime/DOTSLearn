using Jrd.Gameplay.Products;
using Jrd.Gameplay.Products.Component;
using Jrd.MyUtils;
using Jrd.MyUtils.Const;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel.Part
{
    //TODO cache?
    public class ProdLineUI : IBuildingProductionLine
    {
        private readonly VisualElement _container;
        private readonly VisualTreeAsset _itemContainerTemplate;
        private readonly VisualTreeAsset _arrowTemplate;

        public ProdLineUI(VisualElement panel, VisualTreeAsset itemContainerTemplate,
            VisualTreeAsset arrowTemplate)
        {
            _container = panel.Q<VisualElement>(BCPNamesID.ProdLineContainerId);
            _itemContainerTemplate = itemContainerTemplate;
            _arrowTemplate = arrowTemplate;
        }

        public void SetLineInfo(NativeList<ProductData> required,
            NativeList<ProductData> manufactured)
        {
            // TODO all method / refact

            _container.Clear();

            foreach (var reqBuffer in required)
            {
                _container.Add(GetFilledItem(reqBuffer.Name, reqBuffer.Quantity));
            }

            _container.Add(_arrowTemplate.Instantiate());

            foreach (var manBuffer in manufactured)
            {
                _container.Add(GetFilledItem(manBuffer.Name, manBuffer.Quantity));
            }
        }

        private VisualElement GetFilledItem(Product item, int itemCount)
        {
            // TODO all method / refact

            VisualElement template = _itemContainerTemplate.Instantiate();

            var itemContainer = template.Q<VisualElement>(BCPNamesID.ProdLineItemContainerId);
            var itemCountLabel = template.Q<Label>(BCPNamesID.ProdLineItemCountLabelId);

            // Icon //TODO getpath, enum to string?lol?
            var iconPath = GameConst.GoodsIconsPath + item.ToString().ToLower();
            var iconSprite = Utils.LoadFromResources<Sprite>(iconPath, this);

            itemContainer.style.backgroundImage = new StyleBackground(iconSprite);
            itemCountLabel.text = itemCount.ToString();

            return template;
        }
    }

    public interface IBuildingProductionLine
    {
        public void SetLineInfo(NativeList<ProductData> required,
            NativeList<ProductData> manufactured);
    }
}