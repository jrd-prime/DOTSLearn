﻿using System;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Sources.Scripts.UI.PopUpPanels
{
    public class ConfirmationPanelUI : PanelMono
    {
        private Button _cancelButton;
        private Button _applyButton;

        public const string ConfirmationPanelTitle = "Confirmation Title";

        public static ConfirmationPanelUI Instance { private set; get; }
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
            PanelIdName = "confirmation-container";
            PanelTitleIdName = "text";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = Panel.Q<Label>(PanelTitleIdName);
            CancelButton = PanelRoot.Q<Button>("cancel-button");
            ApplyButton = PanelRoot.Q<Button>("apply-button");

            if (Panel != null)
            {
                base.HidePanel();
                IsVisible = false;
            }

            CancelButton.RegisterCallback<ClickEvent>(evt => OnTempBuildCancelled?.Invoke());
            ApplyButton.RegisterCallback<ClickEvent>(evt => OnTempBuildApply?.Invoke());
        }

        public override void SetPanelTitle(string titleText)
        {
            base.SetPanelTitle(ConfirmationPanelTitle);
        }

        protected override void OnCloseButton()
        {
            throw new NotImplementedException();
        }

        public override void ShowPanel()
        {
            base.ShowPanel();
            Panel.experimental.animation
                .Start(
                    new StyleValues { bottom = PanelHeight * -1 },
                    new StyleValues { bottom = BottomMargin },
                    ShowDuration)
                .Ease(Easing.OutElastic)
                .KeepAlive();
        }

        public override void HidePanel()
        {
            Panel.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomMargin },
                    new StyleValues { bottom = PanelHeight * -1 },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => base.HidePanel();
        }
    }
}