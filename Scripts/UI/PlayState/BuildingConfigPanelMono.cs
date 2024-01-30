using UnityEngine;
using UnityEngine.UIElements;

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

            var ToogleCheck = PanelVisualElement.Q<VisualElement>("toggle-check");

            ToogleCheck.RegisterCallback<ClickEvent>(evt =>
            {
                Debug.Log("togg click");
                ToogleCheck.style.backgroundColor = new StyleColor(new Color(74,91,63,1));
            });

            if (PanelVisualElement == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }

        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }

        protected override void OnCloseButton() => HidePanel();
    }
}