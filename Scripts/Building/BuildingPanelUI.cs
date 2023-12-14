using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.EventSystems;
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
        private VisualElement root;


        private VisualTreeAsset UXMLTree;
        [SerializeField] private VisualTreeAsset bt;

        public static event Action<Button> OnBuildingButtonClicked;


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
            UXMLTree = GetComponent<UIDocument>().visualTreeAsset;
            root = GetComponent<UIDocument>().rootVisualElement;
            BuildingsPanelRoot = root;
            BuildingPanel = root.Q<VisualElement>("building-panel");

// LOOK TODO разгребать это
            var a = root.Q<GroupBox>("groupbox");
            a.Clear();
            // var cont = 
            for (int i = 0; i < 4; i++)
            {
                a.Add(bt.Instantiate());
            }

            var buttons = root.Query<Button>();
            buttons.ForEach(RegisterHandler);
            var btns = buttons.ToList();
            for (var i = 0; i < btns.Count; i++)
            {
                btns[i].name = i.ToString();
                btns[i].text = "b-" + i;
            }


            HideElement(BuildingsPanelRoot);
        }

        private void RegisterHandler(Button button)
        {
            button.RegisterCallback<ClickEvent>(
                evt => OnBuildingButtonClicked?.Invoke(evt.currentTarget as Button));
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