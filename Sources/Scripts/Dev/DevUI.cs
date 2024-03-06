using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.Dev
{
    public class DevUI : MonoBehaviour
    {
        public static DevUI Instance { private set; get; }
        private VisualElement _root;

        public Button AddRndProdsToMainStorageButton;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            var mainPanel = _root.Q<VisualElement>("dev-left-panel");

            AddRndProdsToMainStorageButton = mainPanel.Q<Button>("rnd-prods-to-main");
        }
    }
}