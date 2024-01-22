using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd
{
    public class BuildingPanelMono : MonoBehaviour, IVisibleElement
    {
        [SerializeField] private VisualTreeAsset _buildingButtonTemplate;

        private VisualElement _root;

        private VisualElement _buildingPanel;
        private const string BuildingPanelName = "building-panel";
        private const string ButtonsContainerName = "groupbox";
        private static GroupBox _buttonsContainer;

        public static BuildingPanelMono Instance { private set; get; }
        public bool IsVisible { private set; get; }

        private const int ShowDuration = 1;
        private const int HideDuration = 1;
        private const float PanelHeight = 333f;
        private const float BottomMargin = 10f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _buildingPanel = _root.Q<VisualElement>(BuildingPanelName);
            _buttonsContainer = _root.Q<GroupBox>(ButtonsContainerName);


            if (_buildingPanel != null)
            {
                _buildingPanel.style.display = DisplayStyle.None;
                IsVisible = false;
            }

            if (_buildingButtonTemplate == null)
            {
                Debug.LogError("ButtonTemplate not added to script. " + this);
            }
        }

        #region Show/Hide Panel

        public void SetElementVisible(bool value)
        {
            switch (IsVisible)
            {
                case false when value:
                    Show();
                    IsVisible = true;
                    break;
                case true when !value:
                    Hide();
                    IsVisible = false;
                    break;
            }
        }

        public void Show()
        {
            _buildingPanel.style.display = DisplayStyle.Flex;

            _buildingPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = PanelHeight },
                    new StyleValues { bottom = BottomMargin },
                    ShowDuration)
                .Ease(Easing.OutElastic)
                .KeepAlive();
        }

        public void Hide()
        {
            _buildingPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomMargin },
                    new StyleValues { bottom = PanelHeight },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => _buildingPanel.style.display = DisplayStyle.None;
        }

        #endregion
    }
}