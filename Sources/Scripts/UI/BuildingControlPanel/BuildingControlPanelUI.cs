using System;
using Sources.Scripts.CommonData.Product;
using Sources.Scripts.UI.BuildingControlPanel.Part;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.UI.BuildingControlPanel
{
    /// <summary>
    /// <para><b>Is responsible for all communication with parts of the building control panel UI</b></para>
    /// <para><see cref="ProdLineUI"/> - Production line.
    /// <see cref="SpecsUI"/> - Specifications.
    /// <see cref="WarehouseUI"/> - Building warehouse.
    /// <see cref="MainStorageUI"/> - Main storage UI in building control panel with matching products for this type of building.
    /// <see cref="MoveFromStorageTimerUI"/> - Timers and any progress bars.
    /// <see cref="InProductionUI"/> - Production required items UI.
    /// <see cref="ManufacturedUI"/> - Production manufactured items UI</para>
    /// </summary>
    public class BuildingControlPanelUI : PanelMono, IBuildingProductionLine, IBcpSpecs, IBcpMoveFromStorageTimerUI
    {
        #region UI Parts

        public ProdLineUI ProdLineUI { get; private set; }
        public IBcpSpecs SpecsUI { get; private set; }
        public IProductsItemsContainer WarehouseUI { get; private set; }
        public IProductsItemsContainer StorageUI { get; private set; }
        public MoveFromStorageTimerUI MoveFromStorageTimerUI { get; private set; }
        public ProductionTimersUI ProductionTimersUI { get; private set; }
        public IProductsItemsContainer InProductionUI { get; private set; }
        public IProductsItemsContainer ManufacturedUI { get; private set; }

        #endregion

        #region Visual elements

        [SerializeField] private VisualTreeAsset _prodLineItemTemplate;
        [SerializeField] private VisualTreeAsset _prodLineArrowTemplate;
        [SerializeField] private VisualTreeAsset _internalStorageItemTemplate;
        [SerializeField] private VisualTreeAsset _boxItemTemplate;

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

        #region Instance

        public static BuildingControlPanelUI Instance { get; private set; }

        protected void Awake()
        {
            if (Instance == null) Instance = this;
        }

        #endregion

        #region Enable/Disable

        protected void OnEnable()
        {
            PanelIdName = BCPNamesID.PanelId;
            PanelTitleIdName = BCPNamesID.PanelTitleId;
            CloseButtonIdName = BCPNamesID.CloseButtonIdName;

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = Panel.Q<Label>(PanelTitleIdName);
            PanelCloseButton = Panel.Q<Button>(CloseButtonIdName);
            // Buttons
            UpgradeButton = Panel.Q<Button>(BCPNamesID.UpgradeButtonId);
            BuffButton = Panel.Q<Button>(BCPNamesID.BuffButtonId);
            MoveButton = Panel.Q<Button>(BCPNamesID.MoveButtonId);
            LoadButton = Panel.Q<Button>(BCPNamesID.LoadButtonId);
            TakeButton = Panel.Q<Button>(BCPNamesID.TakeButtonId);
            InstantDeliveryButton = Panel.Q<Button>("instant-delivery-button");
            // Labels
            LevelLabel = Panel.Q<Label>(BCPNamesID.LevelLabelId);

            MoveButton.Q<Label>().text = "move to warehouse".ToUpper();

            UpgradeButton.RegisterCallback<ClickEvent>(Callback);
            BuffButton.RegisterCallback<ClickEvent>(Callback);
            MoveButton.RegisterCallback<ClickEvent>(Callback);
            LoadButton.RegisterCallback<ClickEvent>(Callback);
            TakeButton.RegisterCallback<ClickEvent>(Callback);

            var toggleCheck = Panel.Q<VisualElement>("toggle-check");
            toggleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("toggle click");
                toggleCheck.style.backgroundColor = new StyleColor(new Color(74, 91, 63, 1));
            });


            if (Panel == null) return;
            base.HidePanel();
            IsVisible = false;

            if (_prodLineItemTemplate == null || _prodLineArrowTemplate == null || _boxItemTemplate == null)
            {
                throw new NullReferenceException("Add templates to " + this);
            }

            ProdLineUI = new ProdLineUI(Panel, _prodLineItemTemplate, _prodLineArrowTemplate);
            SpecsUI = new SpecsUI(Panel);
            WarehouseUI = new WarehouseUI(Panel, _internalStorageItemTemplate);
            StorageUI = new MainStorageUI(Panel, _internalStorageItemTemplate);
            MoveFromStorageTimerUI = new MoveFromStorageTimerUI(Panel);
            ProductionTimersUI = new ProductionTimersUI(Panel);
            InProductionUI = new InProductionBoxUI(Panel, _boxItemTemplate);
            ManufacturedUI = new ManufacturedBoxUI(Panel, _boxItemTemplate);

            PanelCloseButton.clicked += OnCloseButton;
        }

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
            UpgradeButton.UnregisterCallback<ClickEvent>(Callback);
        }

        #endregion

        protected override void OnCloseButton() => HidePanel();


        private void Callback(ClickEvent evt)
        {
            Debug.Log($"Click + {evt.currentTarget}");
            var a = evt.target as Button;
            var an = a?.experimental.animation.Scale(1.3f, 200);
            if (an != null)
                an.onAnimationCompleted += () => { a.experimental.animation.Scale(1f, 200); };
        }

        #region Set methods

        public void SetLevel(int level) => LevelLabel.text = level.ToString();

        // Production line methods
        public void SetLineInfo(NativeList<ProductData> required, NativeList<ProductData> manufactured) =>
            ProdLineUI.SetLineInfo(required, manufactured);

        // Specs methods
        public void SetSpecName(Spec specName, string value) => SpecsUI.SetSpecName(specName, value);
        public void SetProductivity(float value) => SpecsUI.SetProductivity(value);
        public void SetLoadCapacity(int value) => SpecsUI.SetLoadCapacity(value);
        public void SetStorageCapacity(int value) => SpecsUI.SetStorageCapacity(value);


        public void SetItems(IProductsItemsContainer ui, in NativeList<ProductData> productsData) =>
            ui.SetItems(productsData);

        public void UpdateItemQuantity(object item, int value) => StorageUI.UpdateItemQuantity(item, value);

        // Timer // TODO refact
        public void RunMoveFromStorageTimerAsync(float duration) =>
            MoveFromStorageTimerUI.RunMoveFromStorageTimerAsync(duration);

        public void UpdateProductionTimers(int currentCycle, int maxLoads, int cycle) =>
            ProductionTimersUI.UpdateTimers(currentCycle, maxLoads, cycle);

        #endregion
    }
}