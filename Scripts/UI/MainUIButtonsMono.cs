using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd
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
        private VisualElement _buildingStateButtonContainer;
        public static Button BuildingStateButton;

        public static MainUIButtonsMono Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _buildingStateButtonContainer = _root.Q<VisualElement>("building-state-button-container");
            BuildingStateButton = _root.Q<Button>("building-state-button");
            
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