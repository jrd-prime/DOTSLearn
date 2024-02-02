using Jrd.GameStates.BuildingState.Prefabs;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.PlayState
{
    public class BuildingInfoPanelUIController : PanelMono, IBuildingProductionLine, IBuildingSpecs
    {
        private BuildingProductionLineUIController _productionLineUI;
        private BuildingSpecsUIController _specsUI;

        [SerializeField] private VisualTreeAsset _itemTemplate;
        [SerializeField] private VisualTreeAsset _arrowTemplate;

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

        #endregion

        public static BuildingInfoPanelUIController Instance { private set; get; }

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
            BuildingPanel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = BuildingPanel.Q<Label>(PanelTitleIdName);
            PanelCloseButton = BuildingPanel.Q<Button>(PanelCloseButtonIdName);
            // Buttons
            UpgradeButton = BuildingPanel.Q<Button>(ButtonUpgradeId);
            BuffButton = BuildingPanel.Q<Button>(ButtonBuffId);
            MoveButton = BuildingPanel.Q<Button>(ButtonMoveId);
            LoadButton = BuildingPanel.Q<Button>(ButtonLoadId);
            TakeButton = BuildingPanel.Q<Button>(ButtonTakeId);
            // Labels
            LevelLabel = BuildingPanel.Q<Label>(LevelLabelId);
            BuildingStorageNameLabel = BuildingPanel.Q<Label>(BuildingStorageNameLabelId);
            MainStorageNameLabel = BuildingPanel.Q<Label>(MainStorageNameLabelId);

            UpgradeButton.RegisterCallback<ClickEvent>(Callback);
            BuffButton.RegisterCallback<ClickEvent>(Callback);
            MoveButton.RegisterCallback<ClickEvent>(Callback);
            LoadButton.RegisterCallback<ClickEvent>(Callback);
            TakeButton.RegisterCallback<ClickEvent>(Callback);

            var ToogleCheck = BuildingPanel.Q<VisualElement>("toggle-check");

            ToogleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("togg click");
                ToogleCheck.style.backgroundColor = new StyleColor(new Color(74, 91, 63, 1));
            });

            #endregion

            _productionLineUI = new BuildingProductionLineUIController(BuildingPanel, _itemTemplate, _arrowTemplate);
            _specsUI = new BuildingSpecsUIController(BuildingPanel);

            if (BuildingPanel == null) return;
            base.HidePanel();
            IsVisible = false;

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
    }
}