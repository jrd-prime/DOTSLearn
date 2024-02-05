using System;
using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingControlPanel
{
    public class BuildingControlPanelUI : PanelMono, IBuildingProductionLine, IBuildingSpecs
    {
        private BuildingControlPanelProdLineUI _controlPanelProdLineUI;
        private BuildingControlPanelSpecsUI _controlPanelSpecsUI;
        private BuildingControlPanelWarehouseUI _controlPanelWarehouseUI;

        [SerializeField] private VisualTreeAsset _prodLineItemTemplate;
        [SerializeField] private VisualTreeAsset _prodLineArrowTemplate;
        [SerializeField] private VisualTreeAsset _internalStorageItemTemplate;

        #region VisualElementsVars

        // Buttons
        protected Button TakeButton;
        protected Button LoadButton;
        protected Button MoveButton;
        protected Button BuffButton;
        protected Button UpgradeButton;

        // Common Labels
        protected Label LevelLabel;
        // protected Label BuildingStorageNameLabel;
        // protected string BuildingStorageNameLabelId = "building-storage-name-label";
        // protected Label MainStorageNameLabel;
        // protected string MainStorageNameLabelId = "main-storage-name-label";

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

            _controlPanelProdLineUI =
                new BuildingControlPanelProdLineUI(Panel, _prodLineItemTemplate, _prodLineArrowTemplate);
            _controlPanelSpecsUI = new BuildingControlPanelSpecsUI(Panel);
            _controlPanelWarehouseUI = new BuildingControlPanelWarehouseUI(Panel, _internalStorageItemTemplate);
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
            _controlPanelProdLineUI.SetLineInfo(required, manufactured);

        // Specs methods
        public void SetSpecName(Spec specName, string value) => _controlPanelSpecsUI.SetSpecName(specName, value);
        public void SetProductivity(float value) => _controlPanelSpecsUI.SetProductivity(value);
        public void SetLoadCapacity(int value) => _controlPanelSpecsUI.SetLoadCapacity(value);
        public void SetStorageCapacity(int value) => _controlPanelSpecsUI.SetStorageCapacity(value);
    }
}