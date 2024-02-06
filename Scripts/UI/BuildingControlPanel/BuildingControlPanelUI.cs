using System;
using Jrd.Gameplay.Product;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelUI : PanelMono, IBuildingProductionLine, IBuildingSpecs
    {
        private BuildingControlPanelProdLineUI _productionLineUI;
        private BuildingControlPanelSpecsUI _specsUI;
        private IBuildingControlPanelStorage _warehouseUI;
        private IBuildingControlPanelStorage _storage;

        [SerializeField] private VisualTreeAsset _prodLineItemTemplate;
        [SerializeField] private VisualTreeAsset _prodLineArrowTemplate;
        [SerializeField] private VisualTreeAsset _internalStorageItemTemplate;

        #region VisualElementsVars

        // Buttons
        public Button TakeButton;
        public Button LoadButton;
        public Button MoveButton;
        public Button BuffButton;
        public Button UpgradeButton;

        // Common Labels
        protected Label LevelLabel;

        #endregion

        public static BuildingControlPanelUI Instance { private set; get; }

        protected void Awake()
        {
            if (Instance == null) Instance = this;
        }

        protected void OnEnable()
        {
            #region InitVisualElements

            PanelIdName = BCPNamesID.PanelId;
            PanelTitleIdName = BCPNamesID.PanelTitleId;
            CloseButtonId = BCPNamesID.CloseButtonId;

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = Panel.Q<Label>(PanelTitleIdName);
            PanelCloseButton = Panel.Q<Button>(CloseButtonId);
            // Buttons
            UpgradeButton = Panel.Q<Button>(BCPNamesID.UpgradeButtonId);
            BuffButton = Panel.Q<Button>(BCPNamesID.BuffButtonId);
            MoveButton = Panel.Q<Button>(BCPNamesID.MoveButtonId);
            LoadButton = Panel.Q<Button>(BCPNamesID.LoadButtonId);
            TakeButton = Panel.Q<Button>(BCPNamesID.TakeButtonId);
            // Labels
            LevelLabel = Panel.Q<Label>(BCPNamesID.LevelLabelId);
            // BuildingStorageNameLabel = Panel.Q<Label>(BuildingStorageNameLabelId);
            // MainStorageNameLabel = Panel.Q<Label>(MainStorageNameLabelId);

            UpgradeButton.RegisterCallback<ClickEvent>(Callback);
            BuffButton.RegisterCallback<ClickEvent>(Callback);
            MoveButton.RegisterCallback<ClickEvent>(Callback);
            LoadButton.RegisterCallback<ClickEvent>(Callback);
            TakeButton.RegisterCallback<ClickEvent>(Callback);

            var ToogleCheck = Panel.Q<VisualElement>("toggle-check");

            ToogleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("toggle click");
                ToogleCheck.style.backgroundColor = new StyleColor(new Color(74, 91, 63, 1));
            });

            #endregion


            if (Panel == null) return;
            base.HidePanel();
            IsVisible = false;

            if (_prodLineItemTemplate == null || _prodLineArrowTemplate == null)
            {
                throw new NullReferenceException("Add templates to " + this);
            }

            _productionLineUI =
                new BuildingControlPanelProdLineUI(Panel, _prodLineItemTemplate, _prodLineArrowTemplate);
            _specsUI = new BuildingControlPanelSpecsUI(Panel);
            _warehouseUI = new BuildingControlPanelWarehouseUI(Panel, _internalStorageItemTemplate);
            _storage = new BuildingControlPanelMainStorageUI(Panel, _internalStorageItemTemplate);
            PanelCloseButton.clicked += OnCloseButton;
        }

        public void SetLevel(int level)
        {
            LevelLabel.text = level.ToString();
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

        // ProductionLine methods
        public void SetLineInfo(DynamicBuffer<BuildingRequiredItemsBuffer> required,
            DynamicBuffer<BuildingManufacturedItemsBuffer> manufactured) =>
            _productionLineUI.SetLineInfo(required, manufactured);

        // Specs methods
        public void SetSpecName(Spec specName, string value) => _specsUI.SetSpecName(specName, value);
        public void SetProductivity(float value) => _specsUI.SetProductivity(value);
        public void SetLoadCapacity(int value) => _specsUI.SetLoadCapacity(value);
        public void SetStorageCapacity(int value) => _specsUI.SetStorageCapacity(value);

        // Storage methods
        public void SetStorageItems(NativeList<ProductData> list) => _storage.SetItems(list);
        public void SetWarehouseItems(NativeList<ProductData> list) => _warehouseUI.SetItems(list);

        public void UpdateItemQuantity(object item, int value) =>
            _storage.UpdateItemQuantity(item, value);
    }
}