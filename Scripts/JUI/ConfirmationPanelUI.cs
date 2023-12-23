using Jrd.Utils.Const;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.JUI
{
    public class ConfirmationPanelUI : MonoBehaviour
    {
        public static ConfirmationPanelUI Instance;

        public static VisualElement ApplyPanel;
        public static VisualElement ApplyPanelRoot;
        public static Label ApplyPanelLabel;
        public static Button ApplyPanelCancelButton;
        public static Button ApplyPanelApplyButton;
        private const float BottomHided = -100f;
        private const float BottomShowed = 0f;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;

        private ConfirmationPanelUI()
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
            ApplyPanelRoot = GetComponent<UIDocument>().rootVisualElement;
            // _mouseRaycastToggle = root.Q<Toggle>("mouse-raycast");
            ApplyPanel = ApplyPanelRoot.Q<VisualElement>("apply-panel");
            ApplyPanelLabel = ApplyPanelRoot.Q<Label>("label-text");
            ApplyPanelCancelButton = ApplyPanelRoot.Q<Button>(UIConst.ApplyPanelCancelBtnName);
            ApplyPanelApplyButton = ApplyPanelRoot.Q<Button>(UIConst.ApplyPanelApplyBtnName);

            ApplyPanelRoot.style.display = DisplayStyle.None;
            ApplyPanelRoot.style.bottom = BottomHided;
            
            ApplyPanelCancelButton.clicked += CancelBuilding;
        }

        private void CancelBuilding()
        {
            HideApplyPanel();
        }

        private void OnDisable()
        {
            ApplyPanelCancelButton.clicked -= CancelBuilding;
        }

        public static void ShowApplyPanel()
        {
            ApplyPanelRoot.style.display = DisplayStyle.Flex;
            ApplyPanelRoot.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomHided },
                    new StyleValues { bottom = BottomShowed },
                    ShowDuration)
                .Ease(Easing.OutElastic)
                .KeepAlive();
        }

        public static void HideApplyPanel()
        {
            ApplyPanelRoot.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomShowed },
                    new StyleValues { bottom = BottomHided },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => ApplyPanelRoot.style.display = DisplayStyle.None;
        }
    }
}