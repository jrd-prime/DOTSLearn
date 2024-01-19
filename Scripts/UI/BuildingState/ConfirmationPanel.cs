using System;
using Jrd.DebSet;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.UI.BuildingState
{
    public class ConfirmationPanel : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _confirmationPanel;
        private Button _cancelButton;
        private Button _applyButton;
        private Label _text;

        public static ConfirmationPanel Instance { private set; get; }

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
            _confirmationPanel = _root.Q<VisualElement>("confirmation-container");
            _cancelButton = _root.Q<Button>("cancel-button");
            _applyButton = _root.Q<Button>("apply-button");
            _text = _root.Q<Label>("text");

            if (_confirmationPanel != null) _confirmationPanel.style.display = DisplayStyle.None;
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