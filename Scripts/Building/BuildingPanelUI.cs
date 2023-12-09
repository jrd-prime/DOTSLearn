using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd
{
    public class BuildingPanelUI : MonoBehaviour
    {
        public static BuildingPanelUI Instance;

        public static VisualElement BuildingPanel;
        public static Button BuildingCancel;
        public static Button Building1;
        public static Button Building2;


        private BuildingPanelUI()
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
            BuildingPanel = root.Q<VisualElement>("building-panel");
            BuildingCancel = root.Q<Button>("building-cancel");
            Building1 = root.Q<Button>("building-1");
            Building2 = root.Q<Button>("building-2");
        }
    }
}