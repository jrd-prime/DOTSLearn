using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd
{
    public class BuildingPanelUI : MonoBehaviour
    {
        public static BuildingPanelUI Instance;

        public static VisualElement BuildingPanel;
        public static Button BuildingCancel;
        public static Button Building1;
        public static Button Building2;
        private const float BottomHided = -100f;
        private const float BottomShowed = 0f;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;


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
            var root = GetComponent<UIDocument>().rootVisualElement;
            BuildingsPanelRoot = root;
            BuildingPanel = root.Q<VisualElement>("building-panel");
            BuildingCancel = root.Q<Button>("building-cancel");
            Building1 = root.Q<Button>("building-1");
            Building2 = root.Q<Button>("building-2");

            HideElement(BuildingsPanelRoot);
        }

        private void HideElement(VisualElement e)
        {
            e.style.display = DisplayStyle.None;
        }

        public static void SetRootDisplay(DisplayStyle displayStyle)
        {
            BuildingsPanelRoot.style.display = displayStyle;
        }
        
        public static void ShowEditModePanel()
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

        // BUG on hide panel
        public static void HideEditModePanel()
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