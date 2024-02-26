using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.UI
{
    public class MainUIButtonsMono : MonoBehaviour, IVisibleElement
    {
        private VisualElement _root;
        private VisualElement _confirmationPanel;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;
        private const float PanelHeight = 34f;
        private const float BottomMargin = 10f;

        // Right bottom
        private VisualElement _blueprintsShopButtonContainer;
        private VisualElement _mainStorageButtonContainer;
        public static Button BlueprintsShopButton;
        public static Button MainStorageButton;

        public static MainUIButtonsMono Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _blueprintsShopButtonContainer = _root.Q<VisualElement>("building-state-button-container");
            BlueprintsShopButton = _root.Q<Button>("building-state-button");

            _mainStorageButtonContainer = _root.Q<VisualElement>("main-storage-button-container");
            MainStorageButton = _root.Q<Button>("main-storage-button");

            if (_confirmationPanel != null) _confirmationPanel.style.display = DisplayStyle.None;
        }

        public void SetElementVisible(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void Show()
        {
            throw new System.NotImplementedException();
        }

        public void Hide()
        {
            throw new System.NotImplementedException();
        }
    }
}