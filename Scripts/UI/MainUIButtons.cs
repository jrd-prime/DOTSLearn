using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd
{
    public class MainUIButtons : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _confirmationPanel;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;
        private const float PanelHeight = 34f;
        private const float BottomMargin = 10f;

        // Right bottom
        private VisualElement _buildingStateButtonContainer;
        private Button _buildingStateButton;

        public static MainUIButtons Instance { private set; get; }
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _buildingStateButtonContainer = _root.Q<VisualElement>("building-state-button-container");
            _buildingStateButton = _root.Q<Button>("building-state-button");


            if (_confirmationPanel != null) _confirmationPanel.style.display = DisplayStyle.None;
        }
    }
}