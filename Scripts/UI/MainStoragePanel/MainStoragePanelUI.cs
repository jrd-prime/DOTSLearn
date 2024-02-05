using Jrd.Gameplay.Storage.MainStorage;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.MainStoragePanel
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
            PanelIdName = MSPNamesID.PanelId;
            PanelTitleIdName = MSPNamesID.PanelTitleId;
            CloseButtonId = MSPNamesID.CloseButtonId;

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelCloseButton = Panel.Q<Button>(CloseButtonId);
            ItemsCont = Panel.Q<VisualElement>(MSPNamesID.ItemsContainerId);
            lab = Panel.Q<Label>(MSPNamesID.TestTextLabelId);

            if (Panel == null) return;
            base.HidePanel();
            IsVisible = false;

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

        protected override void OnCloseButton() => SetElementVisible(false);

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }
    }
}