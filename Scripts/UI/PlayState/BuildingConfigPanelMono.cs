using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.PlayState
{
    public class BuildingConfigPanelMono : PanelMono
    {
        // Buttons
        protected Button ButtonTake;
        protected Button ButtonLoad;
        protected Button ButtonMove;
        protected Button ButtonBuff;
        protected Button ButtonUpgrade;

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

            ButtonUpgrade = PanelVisualElement.Q<Button>("btn-upgrade");
            ButtonBuff = PanelVisualElement.Q<Button>("btn-buff");
            ButtonMove = PanelVisualElement.Q<Button>("btn-move");
            ButtonLoad = PanelVisualElement.Q<Button>("btn-load");
            ButtonTake = PanelVisualElement.Q<Button>("btn-take");


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