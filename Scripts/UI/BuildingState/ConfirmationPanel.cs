using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.UI.BuildingState
{
    public class ConfirmationPanel : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _confirmationPanel;
        private Button _cancelButton;
        private Button _applyButton;
        private Label _text;

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _confirmationPanel = _root.Q<VisualElement>("confirmation-panel");
            _cancelButton = _root.Q<Button>("cancel-button");
            _applyButton = _root.Q<Button>("apply-button");
            _text = _root.Q<Label>("text");
        }

        private void Update()
        {
            _text.text = "aaa";
        }
    }
}