using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.DebSet
{
    public class DebSetUI : MonoBehaviour
    {
        public static DebSetUI Instance;
        
        public static Button DebSetApplyButton;
        public static Button DebSetClearLogButton;
        
        public static Label DebSetText;

        public static bool IsMouseRaycast; // TODO переделать
        private Toggle _mouseRaycastToggle;

        private DebSetUI() 
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
            var root = GetComponent<UIDocument>().rootVisualElement;
            _mouseRaycastToggle = root.Q<Toggle>("mouse-raycast");
            DebSetApplyButton = root.Q<Button>("apply-button");
            DebSetClearLogButton = root.Q<Button>("clear-button");
            DebSetText = root.Q<Label>("txt-lab");
        }

        private void FixedUpdate()
        {
            IsMouseRaycast = _mouseRaycastToggle.value;
        }
    }
}