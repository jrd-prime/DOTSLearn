using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.JUI
{
    public class BuildingPanelUI : MonoBehaviour
    {
        public static BuildingPanelUI Instance;

        public static VisualElement BuildingPanel;
        private const float BottomHided = -100f;
        private const float BottomShowed = 0f;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;
        private static VisualElement _root;

        private static GroupBox ButtonsContainer;
        private const string ButtonsContainerName = "groupbox";

        [SerializeField] private VisualTreeAsset _buildingButtonTemplate;
        private static VisualTreeAsset _buttonTemplate;

        public static event Action<Button, int> OnBuildSelected;


        private BuildingPanelUI()
        {
        }

        public static VisualElement BuildingsPanelRoot { get; set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            BuildingsPanelRoot = _root;
            BuildingPanel = _root.Q<VisualElement>("building-panel");
            ButtonsContainer = _root.Q<GroupBox>(ButtonsContainerName);
            BuildingsPanelRoot.style.display = DisplayStyle.None;
            if (_buildingButtonTemplate == null)
            {
                Debug.LogError("ButtonTemplate not added to script. " + this);
            }
            else
            {
                _buttonTemplate = _buildingButtonTemplate;
            }
        }

        public static void InstantiateButtons(int prefabsCount)
        {
            // LOOK TODO разгребать это
            ButtonsContainer.Clear();

            for (var i = 0; i < prefabsCount; i++)
                ButtonsContainer.Add(_buttonTemplate.Instantiate());

            var buttons = _root.Query<Button>();

            var index = 0;
            buttons.ForEach(element =>
            {
                var index1 = index;
                element.name = index1.ToString(); // TODO
                element.text = "b-" + index1; // TODO
                element.RegisterCallback<ClickEvent>(
                    evt => OnBuildSelected?.Invoke(evt.currentTarget as Button, index1));
                ++index;
            });
        }

        public static void SetRootDisplay(DisplayStyle displayStyle)
        {
            BuildingsPanelRoot.style.display = displayStyle;
        }

        public static void ShowApplyPanel()
        {
            BuildingsPanelRoot.style.display = DisplayStyle.Flex;
            BuildingsPanelRoot.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomHided },
                    new StyleValues { bottom = BottomShowed },
                    ShowDuration)
                .Ease(Easing.OutElastic)
                .KeepAlive();
        }

        public static void HideApplyPanel()
        {
            BuildingsPanelRoot.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomShowed },
                    new StyleValues { bottom = BottomHided },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => BuildingsPanelRoot.style.display = DisplayStyle.None;
        }
    }
}