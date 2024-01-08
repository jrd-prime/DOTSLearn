using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.JUI
{
    public class BuildingPanelUI : MonoBehaviour
    {
        public static BuildingPanelUI Instance;

        private const float BottomHided = -100f;
        private const float BottomShowed = 0f;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;
        private int _tempSelectedBuildID;

        private static VisualElement _root;
        public static VisualElement BuildingPanel;
        private static GroupBox _buttonsContainer;
        private const string ButtonsContainerName = "groupbox";
        [SerializeField] private VisualTreeAsset _buildingButtonTemplate;
        private static VisualTreeAsset _buttonTemplate;
        public static VisualElement BuildingsPanelRoot;

        public static event Action<Button, int> OnBuildSelected;

        private BuildingPanelUI()
        {
        }

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
            _buttonsContainer = _root.Q<GroupBox>(ButtonsContainerName);
            BuildingsPanelRoot.style.display = DisplayStyle.None;
            if (_buildingButtonTemplate == null)
            {
                Debug.LogError("ButtonTemplate not added to script. " + this);
            }
            else
            {
                _buttonTemplate = _buildingButtonTemplate;
            }

            _tempSelectedBuildID = -1;
            OnBuildSelected += BuildSelected;
            ConfirmationPanelUI.ApplyPanelCancelButton.clicked += CancelBuilding;
        }

        private void CancelBuilding()
        {
            SetButtonEnabled(_tempSelectedBuildID, true);
            _tempSelectedBuildID = -1; // reset temp id
        }

        private void OnDisable()
        {
            OnBuildSelected -= BuildSelected;
            ConfirmationPanelUI.ApplyPanelCancelButton.clicked -= CancelBuilding;
        }

        private void BuildSelected(Button button, int index)
        {
            // here + buildingStateSystem
            if (_tempSelectedBuildID < 0) // temp not set
            {
                SetButtonEnabled(index, false);
                _tempSelectedBuildID = index;
            }
            else if (_tempSelectedBuildID != index) // temp != index
            {
                SetButtonEnabled(index, false);
                SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBuildID = index;
            }
            else // temp = index
            {
                Debug.LogWarning("We have a problem with enable/disable buttons in BuildPanelUI." + this);
            }
        }


        public static void InstantiateButtons(int prefabsCount, NativeList<FixedString32Bytes> names)
        {
            // LOOK TODO разгребать это
            _buttonsContainer.Clear();

            for (var i = 0; i < prefabsCount; i++)
                _buttonsContainer.Add(_buttonTemplate.Instantiate());

            var buttons = _root.Query<Button>();

            var index = 0;
            buttons.ForEach(element =>
            {
                var index1 = index;
                element.name = index1.ToString(); // TODO
                element.text = names.ElementAt(index1).ToString(); // TODO
                element.RegisterCallback<ClickEvent>(
                    evt => OnBuildSelected?.Invoke(evt.currentTarget as Button, index1));
                ++index;
            });
        }

        public static void SetRootDisplay(DisplayStyle displayStyle)
        {
            BuildingsPanelRoot.style.display = displayStyle;
        }

        public static void SetButtonEnabled(int id, bool enabled)
        {
            _root.Query<Button>().AtIndex(id).SetEnabled(enabled);
        }

        public static void ShowBPanel()
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

        public static void HideBPanel()
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