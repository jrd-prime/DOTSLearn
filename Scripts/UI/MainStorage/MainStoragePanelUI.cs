using Jrd.Gameplay.Storage.MainStorage;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.MainStorage
{
    public class MainStoragePanelUI : PanelMono
    {
        protected VisualElement ItemsCont;
        protected Label lab;
        public static MainStoragePanelUI Instance { private set; get; }

        protected void Awake()
        {
            if (Instance == null) Instance = this;
        }

        protected void OnEnable()
        {
            PanelIdName = "main-storage-panel";
            // PanelTitleIdName = "panel-title";
            CloseButtonId = "close-button";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            // PanelTitleLabel = BuildingPanel.Q<Label>(PanelTitleIdName);
            PanelCloseButton = Panel.Q<Button>(CloseButtonId);
            ItemsCont = Panel.Q<VisualElement>("items-cont");
            lab = Panel.Q<Label>("text-label");

            // if (Panel == null) return;
            // base.HidePanel();
            // IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }

        public void SetTestItems(MainStorageData itemsList)
        {
            // ItemsCont.Clear();


            lab.text = "";
            foreach (var keyValue in itemsList.Values)
            {
                if (keyValue.Value == -1) continue;
                lab.text += "\n" + keyValue.Key + " / " + keyValue.Value;
                Debug.LogWarning(keyValue.Key + " / " + keyValue.Value);
            }
        }

        protected override void OnCloseButton() => HidePanel();

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }
    }
}