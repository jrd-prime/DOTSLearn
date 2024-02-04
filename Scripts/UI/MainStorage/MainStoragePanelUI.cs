using UnityEngine.UIElements;

namespace Jrd.UI.MainStorage
{
    public class MainStoragePanelUI : PanelMono
    {
        protected VisualElement ItemsCont;
        public static MainStoragePanelUI Instance { private set; get; }

        protected void Awake()
        {
            if (Instance == null) Instance = this;
        }

        protected void OnEnable()
        {
            PanelIdName = "main-storage-panel";
            // PanelTitleIdName = "panel-title";
            PanelCloseButtonIdName = "close-button";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            // PanelTitleLabel = BuildingPanel.Q<Label>(PanelTitleIdName);
            PanelCloseButton = Panel.Q<Button>(PanelCloseButtonIdName);
            ItemsCont = Panel.Q<VisualElement>("item-cont");

            // if (Panel == null) return;
            // base.HidePanel();
            // IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }

        public void SetTestItems(object itemsList)
        {
            ItemsCont.Clear();
            var lab = ItemsCont.Q<Label>("lab");

            lab.text = "test";
        }

        protected override void OnCloseButton() => HidePanel();

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }
    }
}