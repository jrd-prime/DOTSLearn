using System;
using Jrd.UI.BuildingState;
using Unity.Entities;
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
        public static Button BModeButton;
        public static Button BModeButtonOff;
        public static Button Toff;
        public static Button Ton;

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
            BModeButton = root.Q<Button>("bmode-button");
            BModeButtonOff = root.Q<Button>("bmodeoff-button");
            Toff = root.Q<Button>("off-b");
            Ton = root.Q<Button>("on-b");

            Ton.clicked += () =>
            {
                BuildingPanel.Instance.Show();
                ConfirmationPanel.Instance.Show();
            };
            Toff.clicked += () =>
            {
                BuildingPanel.Instance.Hide();
                ConfirmationPanel.Instance.Hide();
            };
        }


        private void FixedUpdate()
        {
            IsMouseRaycast = _mouseRaycastToggle.value;
        }

        private void OnDisable()
        {
            Ton.clicked -= ConfirmationPanel.Instance.Show;
            Toff.clicked -= ConfirmationPanel.Instance.Hide;
        }
    }
}