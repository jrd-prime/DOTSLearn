using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.Goods;
using Jrd.Utils.Const;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    //TODO cache?
    public class BuildingControlPanelProdLineUI : IBuildingProductionLine
    {
        private readonly VisualElement _container;
        private const string ContainerId = "production-line-cont";
        private const string ItemContainerId = "prod-line-item-cont";
        public const string ItemCountLabelId = "prod-line-item-count";
        private readonly VisualTreeAsset _itemContainerTemplate;
        private readonly VisualTreeAsset _arrowTemplate;

        public BuildingControlPanelProdLineUI(VisualElement panel, VisualTreeAsset itemContainerTemplate,
            VisualTreeAsset arrowTemplate)
        {
            _container = panel.Q<VisualElement>(ContainerId);
            _itemContainerTemplate = itemContainerTemplate;
            _arrowTemplate = arrowTemplate;
        }

        public void SetLineInfo(DynamicBuffer<BuildingRequiredItemsBuffer> required,
            DynamicBuffer<BuildingManufacturedItemsBuffer> manufactured)
        {
            // TODO all method / refact

            _container.Clear();

            foreach (var reqBuffer in required)
            {
                _container.Add(GetFilledItem(reqBuffer._item, reqBuffer._count));
            }

            _container.Add(_arrowTemplate.Instantiate());

            foreach (var manBuffer in manufactured)
            {
                _container.Add(GetFilledItem(manBuffer._item, manBuffer._count));
            }
        }

        private VisualElement GetFilledItem(GoodsEnum item, int itemCount)
        {
            // TODO all method / refact

            VisualElement template = _itemContainerTemplate.Instantiate();

            var itemContainer = template.Q<VisualElement>(ItemContainerId);
            var itemCountLabel = template.Q<Label>(ItemCountLabelId);

            // Icon //TODO getpath, enum to string?lol?
            var iconPath = GameConst.GoodsIconsPath + item.ToString().ToLower();
            var iconSprite = Utils.Utils.LoadFromResources<Sprite>(iconPath, this);

            itemContainer.style.backgroundImage = new StyleBackground(iconSprite);
            itemCountLabel.text = itemCount.ToString();

            return template;
        }
    }

    public interface IBuildingProductionLine
    {
        public void SetLineInfo(DynamicBuffer<BuildingRequiredItemsBuffer> required,
            DynamicBuffer<BuildingManufacturedItemsBuffer> manufactured);
    }
}