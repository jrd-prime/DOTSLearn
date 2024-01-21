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

        private const int ShowDuration = 1000;
        private const int HideDuration = 500;
        private const float PanelHeight = 34f;
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


            if (_buildingPanel != null) _buildingPanel.style.display = DisplayStyle.None;
            if (_buildingButtonTemplate == null)
            {
                Debug.LogError("ButtonTemplate not added to script. " + this);
            }
        }

        public void ShowElement(bool value)
        {
            _buildingPanel.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
            // TODO set animations
        }

        public void Show()
        {
            Debug.Log("show ui");
            _buildingPanel.style.display = DisplayStyle.Flex;
            _buildingPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = PanelHeight * -1 },
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
                    new StyleValues { bottom = PanelHeight * -1 },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () =>
                _buildingPanel.style.display = DisplayStyle.None;
        }
    }
}