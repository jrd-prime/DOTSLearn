using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd
{
    public class BuildingPanel : MonoBehaviour
    {
        private VisualElement _root;

        private VisualElement _buildingPanel;
        private const string ButtonsContainerName = "groupbox";
        private static GroupBox _buttonsContainer;

        public static BuildingPanel Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _buildingPanel = _root.Q<VisualElement>("building-panel");
            _buttonsContainer = _root.Q<GroupBox>(ButtonsContainerName);


            if (_buildingPanel != null) _buildingPanel.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            // TODO anim
            _buildingPanel.style.display = DisplayStyle.Flex;
            // _buildingPanel.experimental.animation
            //     .Start(
            //         new StyleValues { bottom = PanelHeight * -1 },
            //         new StyleValues { bottom = BottomMargin },
            //         ShowDuration)
            //     .Ease(Easing.OutElastic)
            //     .KeepAlive();
        }

        public void Hide()
        {
            // TODO anim
            // _buildingPanel.experimental.animation
            //     .Start(
            //         new StyleValues { bottom = BottomMargin },
            //         new StyleValues { bottom = PanelHeight * -1 },
            //         HideDuration)
            //     .Ease(Easing.InQuad)
            //     .KeepAlive()
            // .onAnimationCompleted = () =>
            _buildingPanel.style.display = DisplayStyle.None;
        }
    }
}