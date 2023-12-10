using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd.JUI.EditModeUI
{
    public class EditModeUI : MonoBehaviour
    {
        public static EditModeUI Instance;

        public static VisualElement EditModePanel;
        public static VisualElement EditModeRoot;
        public static Button EditModeCancelButton;
        private const float BottomHided = -100f;
        private const float BottomShowed = 0f;
        private const int ShowDuration = 1000;
        private const int HideDuration = 500;

        //
        // public static Label DebSetText;
        //
        // public static bool IsMouseRaycast; // TODO переделать
        // private Toggle _mouseRaycastToggle;

        private EditModeUI()
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
            EditModeRoot = GetComponent<UIDocument>().rootVisualElement;
            // _mouseRaycastToggle = root.Q<Toggle>("mouse-raycast");
            EditModePanel = EditModeRoot.Q<VisualElement>("edit-mode-panel");
            EditModeCancelButton = EditModeRoot.Q<Button>("cancel-button");
            // DebSetText = root.Q<Label>("txt-lab");

            EditModeRoot.style.display = DisplayStyle.None;
            EditModeRoot.style.bottom = BottomHided;
        }

        private void FixedUpdate()
        {
            // IsMouseRaycast = _mouseRaycastToggle.value;
        }

        public static void ShowEditModePanel()
        {
            EditModeRoot.style.display = DisplayStyle.Flex;
            EditModeRoot.experimental.animation
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
            EditModeRoot.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomShowed },
                    new StyleValues { bottom = BottomHided },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => EditModeRoot.style.display = DisplayStyle.None;
        }
    }
}