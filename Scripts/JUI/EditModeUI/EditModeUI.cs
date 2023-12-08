using UnityEngine;
using UnityEngine.UIElements;

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
    }
}