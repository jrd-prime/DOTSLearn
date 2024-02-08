﻿using Jrd.Gameplay.Products;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.MyUtils;
using Jrd.MyUtils.Const;
using Jrd.ScriptableObjects;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    //TODO cache?
    public class BuildingControlPanelProdLineUI : IBuildingProductionLine
    {
        private readonly VisualElement _container;
        private readonly VisualTreeAsset _itemContainerTemplate;
        private readonly VisualTreeAsset _arrowTemplate;

        public BuildingControlPanelProdLineUI(VisualElement panel, VisualTreeAsset itemContainerTemplate,
            VisualTreeAsset arrowTemplate)
        {
            _container = panel.Q<VisualElement>(BCPNamesID.ProdLineContainerId);
            _itemContainerTemplate = itemContainerTemplate;
            _arrowTemplate = arrowTemplate;
        }

        public void SetLineInfo(NativeList<ProductionProductData> required,
            NativeList<ProductionProductData> manufactured)
        {
            // TODO all method / refact

            _container.Clear();

            foreach (var req in required)
            {
                Debug.LogWarning(req._productName);
            }

            foreach (var reqBuffer in required)
            {
                _container.Add(GetFilledItem(reqBuffer._productName, reqBuffer._quantity));
            }

            _container.Add(_arrowTemplate.Instantiate());

            foreach (var manBuffer in manufactured)
            {
                _container.Add(GetFilledItem(manBuffer._productName, manBuffer._quantity));
            }
        }

        private VisualElement GetFilledItem(Product item, int itemCount)
        {
            // TODO all method / refact

            Debug.LogWarning(item + " = " + itemCount);
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
        public void SetLineInfo(NativeList<ProductionProductData> required,
            NativeList<ProductionProductData> manufactured);
    }
}