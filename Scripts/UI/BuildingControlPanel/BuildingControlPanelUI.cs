﻿using System;
using Jrd.Gameplay.Products;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    /// <summary>
    /// <para><b>Is responsible for all communication with parts of the building control panel UI</b></para>
    /// <para><see cref="BuildingControlPanelProdLineUI"/> Production line</para>
    /// <para><see cref="BuildingControlPanelSpecsUI"/> Specifications</para>
    /// <para><see cref="BuildingControlPanelWarehouseUI"/> Building warehouse</para>
    /// <para><see cref="BuildingControlPanelMainStorageUI"/> Main storage UI in building control panel with matching products for this type of building</para>
    /// <para><see cref="BuildingControlPanelTimerUI"/> Timers and any progress bars</para>
    /// </summary>
    public class BuildingControlPanelUI : PanelMono, IBuildingProductionLine, IBuildingSpecs, IBuildingTimer
    {
        private BuildingControlPanelProdLineUI _productionLineUI;
        private BuildingControlPanelSpecsUI _specsUI;
        private IBuildingControlPanelStorage _warehouseUI;
        private IBuildingControlPanelStorage _storage;
        private BuildingControlPanelTimerUI _timer;

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
        public Button InstantDeliveryButton;

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
            InstantDeliveryButton = Panel.Q<Button>("instant-delivery-button");
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
            _timer = new BuildingControlPanelTimerUI(Panel);
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
        public void SetLineInfo(NativeList<ProductionProductData> required,
            NativeList<ProductionProductData> manufactured) =>
            _productionLineUI.SetLineInfo(required, manufactured);

        // Specs methods
        public void SetSpecName(Spec specName, string value) => _specsUI.SetSpecName(specName, value);
        public void SetProductivity(float value) => _specsUI.SetProductivity(value);
        public void SetLoadCapacity(int value) => _specsUI.SetLoadCapacity(value);
        public void SetStorageCapacity(int value) => _specsUI.SetStorageCapacity(value);

        // Timer // TODO refact
        public void SetTimerText(float max, float value) => _timer.SetTimerText(max, value);

        // Storage methods
        public void SetStorageItems(NativeList<ProductData> list) => _storage.SetItems(list);
        public void SetWarehouseItems(NativeList<ProductData> list) => _warehouseUI.SetItems(list);

        public void UpdateItemQuantity(object item, int value) =>
            _storage.UpdateItemQuantity(item, value);
    }
}