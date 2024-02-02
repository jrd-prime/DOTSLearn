using System;
using Jrd.GameStates.BuildingState.Prefabs;
using Jrd.Goods;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Jrd.PlayState
{
    public class BuildingConfigPanelMono : PanelMono
    {
        #region VisualElementsVars

        // Buttons
        protected Button TakeButton;
        protected Button LoadButton;
        protected Button MoveButton;
        protected Button BuffButton;
        protected Button UpgradeButton;
        protected string ButtonUpgradeId = "btn-upgrade";
        protected string ButtonBuffId = "btn-buff";
        protected string ButtonMoveId = "btn-move";
        protected string ButtonLoadId = "btn-load";
        protected string ButtonTakeId = "btn-take";

        // Common Labels
        protected Label LevelLabel;
        protected string LevelLabelId = "lvl-label";
        protected Label BuildingStorageNameLabel;
        protected string BuildingStorageNameLabelId = "building-storage-name-label";
        protected Label MainStorageNameLabel;
        protected string MainStorageNameLabelId = "main-storage-name-label";
        protected Label StatConvLabel;
        protected string StatConvLabelId = "stat-conv-label";

        // Production Line
        protected VisualElement ProdLineContainer;
        protected string ProdLineContainerId = "production-line-cont";
        [SerializeField] private VisualTreeAsset _prodLineItemContainerTemplate;
        [SerializeField] private VisualTreeAsset _prodLineArrowTemplate;


        // Stat Labels
        protected Label StatConvIntLabel;
        protected Label StatLoadLabel;
        protected Label StatLoadIntLabel;
        protected Label StatStorageLabel;
        protected Label StatStorageIntLabel;
        protected string StatConvIntLabelIdName = "stat-conv-int-label";
        protected string StatLoadLabelIdName = "stat-load-label";
        protected string StatLoadIntLabelIdName = "stat-load-int-label";
        protected string StatStorageLabelIdName = "stat-storage-label";
        protected string StatStorageIntLabelIdName = "stat-storage-int-label";

        #endregion

        public static BuildingConfigPanelMono Instance { private set; get; }

        protected void Awake()
        {
            if (Instance == null) Instance = this;
        }

        protected void OnEnable()
        {
            #region InitVisualElements

            PanelIdName = "building-config-panel";
            PanelTitleIdName = "panel-title";
            PanelCloseButtonIdName = "close-button";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            PanelVisualElement = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = PanelVisualElement.Q<Label>(PanelTitleIdName);
            PanelCloseButton = PanelVisualElement.Q<Button>(PanelCloseButtonIdName);
            // Buttons
            UpgradeButton = PanelVisualElement.Q<Button>(ButtonUpgradeId);
            BuffButton = PanelVisualElement.Q<Button>(ButtonBuffId);
            MoveButton = PanelVisualElement.Q<Button>(ButtonMoveId);
            LoadButton = PanelVisualElement.Q<Button>(ButtonLoadId);
            TakeButton = PanelVisualElement.Q<Button>(ButtonTakeId);
            // Production Line
            ProdLineContainer = PanelVisualElement.Q<VisualElement>(ProdLineContainerId);
            // Labels
            LevelLabel = PanelVisualElement.Q<Label>(LevelLabelId);
            BuildingStorageNameLabel = PanelVisualElement.Q<Label>(BuildingStorageNameLabelId);
            MainStorageNameLabel = PanelVisualElement.Q<Label>(MainStorageNameLabelId);
            // Stat Labels
            StatConvLabel = PanelVisualElement.Q<Label>(StatConvLabelId);
            StatConvIntLabel = PanelVisualElement.Q<Label>(StatConvIntLabelIdName);
            StatLoadLabel = PanelVisualElement.Q<Label>(StatLoadLabelIdName);
            StatLoadIntLabel = PanelVisualElement.Q<Label>(StatLoadIntLabelIdName);
            StatStorageLabel = PanelVisualElement.Q<Label>(StatStorageLabelIdName);
            StatStorageIntLabel = PanelVisualElement.Q<Label>(StatStorageIntLabelIdName);

            UpgradeButton.RegisterCallback<ClickEvent>(Callback);
            BuffButton.RegisterCallback<ClickEvent>(Callback);
            MoveButton.RegisterCallback<ClickEvent>(Callback);
            LoadButton.RegisterCallback<ClickEvent>(Callback);
            TakeButton.RegisterCallback<ClickEvent>(Callback);

            var ToogleCheck = PanelVisualElement.Q<VisualElement>("toggle-check");

            ToogleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("togg click");
                ToogleCheck.style.backgroundColor = new StyleColor(new Color(74, 91, 63, 1));
            });

            #endregion

            if (PanelVisualElement == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }


        public void SetProductionLineInfo(DynamicBuffer<BuildingRequiredItemsBuffer> requiredItemsBuffers,
            DynamicBuffer<BuildingManufacturedItemsBuffer> manufacturedItemsBuffers)
        {
            // TODO all method / refact

            ProdLineContainer.Clear();

            foreach (var reqBuffer in requiredItemsBuffers)
            {
                ProdLineContainer.Add(GetFilledItemVisualElementForProductionLine(reqBuffer._item, reqBuffer._count));
            }

            ProdLineContainer.Add(_prodLineArrowTemplate.Instantiate());

            foreach (var manBuffer in manufacturedItemsBuffers)
            {
                ProdLineContainer.Add(GetFilledItemVisualElementForProductionLine(manBuffer._item, manBuffer._count));
            }
        }

        private VisualElement GetFilledItemVisualElementForProductionLine(GoodsEnum item, int itemCount)
        {
            // TODO all method / refact

            VisualElement template = _prodLineItemContainerTemplate.Instantiate();

            var prodLineItemContainer = template.Q<VisualElement>("prod-line-item-cont");
            var prodLineItemCountLabel = template.Q<Label>("prod-line-item-count");

            // Icon //TODO getpath, enum to string?lol?
            var iconPath = GoodsIconsPath + item.ToString().ToLower();
            var iconSprite = Resources.Load<Sprite>(iconPath);
            Debug.LogWarning(iconSprite);

            if (iconSprite == null)
            {
                throw new NullReferenceException($"Unable to load the icon sprite. {iconPath} \n" + this);
            }

            prodLineItemContainer.style.backgroundImage = new StyleBackground(iconSprite);
            prodLineItemCountLabel.text = itemCount.ToString();

            return template;
        }

        public void SetStatNames(string manufacturedItemName)
        {
            StatConvLabel.text = $"{manufacturedItemName} / hour";
            StatLoadLabel.text = $"{manufacturedItemName} / load";
            StatStorageLabel.text = $"Max {manufacturedItemName} in storage";
        }

        public void SetLevel(int level)
        {
            LevelLabel.text = level.ToString();
        }

        public void SetSpeed(float buildingDataSpeed)
        {
            StatConvIntLabel.text = buildingDataSpeed.ToString();
        }

        public void SetLoadCapacity(int buildingDataLoadCapacity)
        {
            StatLoadIntLabel.text = buildingDataLoadCapacity.ToString();
        }

        public void SetMaxStorage(int buildingDataMaxStorage)
        {
            StatStorageIntLabel.text = buildingDataMaxStorage.ToString();
        }

        private void Callback(ClickEvent evt)
        {
            Debug.Log($"Click + {evt.currentTarget}");
            var a = evt.target as Button;
            var an = a?.experimental.animation.Scale(1.3f, 200);
            if (an != null)
                an.onAnimationCompleted += () => { a.experimental.animation.Scale(1f, 200); };
        }

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
            UpgradeButton.UnregisterCallback<ClickEvent>(Callback);
        }

        protected override void OnCloseButton() => HidePanel();
    }
}