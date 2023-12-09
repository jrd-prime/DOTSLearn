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
        }

        private void FixedUpdate()
        {
            // IsMouseRaycast = _mouseRaycastToggle.value;
        }

        public static void ShowEditModePanel()
        {
            var root = EditModeRoot;
            if (root.style.display == DisplayStyle.Flex) return;
            root.style.display = DisplayStyle.Flex;
            var showEditMenuPanelAnimation = root.experimental.animation.Start(
                new StyleValues { bottom = -100f },
                new StyleValues { bottom = 0f }, 1000).Ease(Easing.OutElastic).KeepAlive();
        }

        // BUG on hide panel
        public static void HideEditModePanel()
        {
            if (EditModeRoot.style.display == DisplayStyle.None) return;

            var hideEditModePanelAnimation = EditModePanel.experimental.animation.Start(
                new StyleValues { bottom = 0f },
                new StyleValues { bottom = -100f }, 500).Ease(Easing.InQuad).KeepAlive();
            hideEditModePanelAnimation.onAnimationCompleted =
                () => EditModeRoot.style.display = DisplayStyle.None;
            
            // var root = EditModeRoot;
            // if (root.style.display == DisplayStyle.None) return;
            // var hideEditModePanelAnimation = EditModePanel.experimental.animation.Start(
            //     new StyleValues { bottom = 0f },
            //     new StyleValues { bottom = -100f }, 500).Ease(Easing.InSine).KeepAlive();
            // hideEditModePanelAnimation.onAnimationCompleted =
            //     () => root.style.display = DisplayStyle.None;
        }
    }
}