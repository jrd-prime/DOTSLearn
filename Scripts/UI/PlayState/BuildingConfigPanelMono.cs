using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.PlayState
{
    public class BuildingConfigPanelMono : PanelMono
    {
        public static BuildingConfigPanelMono Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            PanelIdName = "building-config-panel";
            PanelTitleIdName = "panel-title";
            PanelCloseButtonIdName = "close-button";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            PanelVisualElement = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = PanelVisualElement.Q<Label>(PanelTitleIdName);
            PanelCloseButton = PanelVisualElement.Q<Button>(PanelCloseButtonIdName);

            var ButtonUpgrade = PanelVisualElement.Q<Button>("btn-upgrade");
            var ButtonBuff = PanelVisualElement.Q<Button>("btn-buff");
            var ButtonMove = PanelVisualElement.Q<Button>("btn-move");
            var ButtonLoad = PanelVisualElement.Q<Button>("btn-load");
            var ButtonTake = PanelVisualElement.Q<Button>("btn-take");


            ButtonUpgrade.RegisterCallback<ClickEvent>(Callback);

            ButtonBuff.RegisterCallback<ClickEvent>(Callback);
            ButtonMove.RegisterCallback<ClickEvent>(Callback);
            ButtonLoad.RegisterCallback<ClickEvent>(Callback);
            ButtonTake.RegisterCallback<ClickEvent>(Callback);

            var ToogleCheck = PanelVisualElement.Q<VisualElement>("toggle-check");

            ToogleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("togg click");
                ToogleCheck.style.backgroundColor = new StyleColor(new Color(74, 91, 63, 1));
            });

            if (PanelVisualElement == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }

        private void Callback(ClickEvent evt)
        {
            Debug.Log($"Click + {evt.currentTarget}");
            var a = evt.target as Button;
            var an = a?.experimental.animation.Scale(1.3f, 200);
            if (an != null)
                an.onAnimationCompleted += () => { a.experimental.animation.Scale(1f, 200); };
        }

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }

        protected override void OnCloseButton() => HidePanel();
    }
}