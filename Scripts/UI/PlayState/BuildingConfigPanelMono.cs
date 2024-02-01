using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.PlayState
{
    public class BuildingConfigPanelMono : PanelMono
    {
        // Buttons
        protected Button ButtonTake;
        protected Button ButtonLoad;
        protected Button ButtonMove;
        protected Button ButtonBuff;
        protected Button ButtonUpgrade;
        protected string ButtonUpgradeIdName = "btn-upgrade";
        protected string ButtonBuffIdName = "btn-buff";
        protected string ButtonMoveIdName = "btn-move";
        protected string ButtonLoadIdName = "btn-load";
        protected string ButtonTakeIdName = "btn-take";

        // Labels
        protected Label LevelLabel;
        protected Label BuildingStorageNameLabel;
        protected Label MainStorageNameLabel;
        protected Label StatConvLabel;
        protected string LevelLabelIdName = "lvl-label";
        protected string BuildingStorageNameLabelIdName = "building-storage-name-label";
        protected string MainStorageNameLabelIdName = "main-storage-name-label";
        protected string StatConvLabelIdName = "stat-conv-label";

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

        public static BuildingConfigPanelMono Instance { private set; get; }

        protected void Awake()
        {
            if (Instance == null) Instance = this;
        }

        protected void OnEnable()
        {
            PanelIdName = "building-config-panel";
            PanelTitleIdName = "panel-title";
            PanelCloseButtonIdName = "close-button";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            PanelVisualElement = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = PanelVisualElement.Q<Label>(PanelTitleIdName);
            PanelCloseButton = PanelVisualElement.Q<Button>(PanelCloseButtonIdName);
            // Buttons
            ButtonUpgrade = PanelVisualElement.Q<Button>(ButtonUpgradeIdName);
            ButtonBuff = PanelVisualElement.Q<Button>(ButtonBuffIdName);
            ButtonMove = PanelVisualElement.Q<Button>(ButtonMoveIdName);
            ButtonLoad = PanelVisualElement.Q<Button>(ButtonLoadIdName);
            ButtonTake = PanelVisualElement.Q<Button>(ButtonTakeIdName);
            // Labels
            LevelLabel = PanelVisualElement.Q<Label>(LevelLabelIdName);
            BuildingStorageNameLabel = PanelVisualElement.Q<Label>(BuildingStorageNameLabelIdName);
            MainStorageNameLabel = PanelVisualElement.Q<Label>(MainStorageNameLabelIdName);
            // Stat Labels
            StatConvLabel = PanelVisualElement.Q<Label>(StatConvLabelIdName);
            StatConvIntLabel = PanelVisualElement.Q<Label>(StatConvIntLabelIdName);
            StatLoadLabel = PanelVisualElement.Q<Label>(StatLoadLabelIdName);
            StatLoadIntLabel = PanelVisualElement.Q<Label>(StatLoadIntLabelIdName);
            StatStorageLabel = PanelVisualElement.Q<Label>(StatStorageLabelIdName);
            StatStorageIntLabel = PanelVisualElement.Q<Label>(StatStorageIntLabelIdName);

            ButtonUpgrade.RegisterCallback<ClickEvent>(Callback);
            ButtonBuff.RegisterCallback<ClickEvent>(Callback);
            ButtonMove.RegisterCallback<ClickEvent>(Callback);
            ButtonLoad.RegisterCallback<ClickEvent>(Callback);
            ButtonTake.RegisterCallback<ClickEvent>(Callback);

            var ToogleCheck = PanelVisualElement.Q<VisualElement>("toggle-check");

            ToogleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("togg click");
                ToogleCheck.style.backgroundColor = new StyleColor(new Color(74, 91, 63, 1));
            });

            if (PanelVisualElement == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
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
            Debug.Log($"lvl = {level}");
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
            ButtonUpgrade.UnregisterCallback<ClickEvent>(Callback);
        }

        protected override void OnCloseButton() => HidePanel();
    }
}