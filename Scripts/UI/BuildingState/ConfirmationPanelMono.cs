using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.UI.BuildingState
{
    public class ConfirmationPanelMono : MonoBehaviour, IVisibleElement
    {
        private VisualElement _root;
        private VisualElement _confirmationPanel;
        private Button _cancelButton;
        private Button _applyButton;

        private const string ConfirmationPanelName = "confirmation-container";
        private const string ConfirmationPanelTitleName = "text";
        public const string ConfirmationPanelTitle = "Build";

        public static ConfirmationPanelMono Instance { private set; get; }
        public bool IsVisible { private set; get; }

        public Label Title { get; set; }
        public Button CancelButton { get; private set; }
        public Button ApplyButton { get; private set; }

        private const int ShowDuration = 1000;
        private const int HideDuration = 500;
        private const float PanelHeight = 34f;
        private const float BottomMargin = 10f;

        public static event Action OnTempBuildCancelled;
        public static event Action OnTempBuildApply;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _confirmationPanel = _root.Q<VisualElement>(ConfirmationPanelName);
            CancelButton = _root.Q<Button>("cancel-button");
            ApplyButton = _root.Q<Button>("apply-button");
            Title = _root.Q<Label>(ConfirmationPanelTitleName);

            if (_confirmationPanel != null)
            {
                _confirmationPanel.style.display = DisplayStyle.None;
                IsVisible = false;
            }

            CancelButton.RegisterCallback<ClickEvent>(evt => OnTempBuildCancelled?.Invoke());
            ApplyButton.RegisterCallback<ClickEvent>(evt => OnTempBuildApply?.Invoke());
        }

        private void SetPanelTitle(string titleText)
        {
            _confirmationPanel.Q<Label>(ConfirmationPanelTitleName).text = titleText.ToUpper();
        }

        public void SetElementVisible(bool value)
        {
            switch (IsVisible)
            {
                case false when value:
                    SetPanelTitle(ConfirmationPanelTitle);
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
            _confirmationPanel.style.display = DisplayStyle.Flex;
            _confirmationPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = PanelHeight * -1 },
                    new StyleValues { bottom = BottomMargin },
                    ShowDuration)
                .Ease(Easing.OutElastic)
                .KeepAlive();
        }

        public void Hide()
        {
            _confirmationPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomMargin },
                    new StyleValues { bottom = PanelHeight * -1 },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => _confirmationPanel.style.display = DisplayStyle.None;
        }
    }
}