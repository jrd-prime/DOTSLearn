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

            if (PanelVisualElement == null) return;
            base.HidePanel();
            IsVisible = false;
            
            PanelCloseButton.clicked += OnCloseButton;
        }
        
        private void OnDisable()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }

        protected override void OnCloseButton()
        {
            Debug.Log("click");
            HidePanel();
        }
    }
}