using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd
{
    public class BuildingPanelMono : MonoBehaviour, IVisibleElement
    {
        [SerializeField] private VisualTreeAsset _buildingCardTemplate;

        private VisualElement _root;

        private VisualElement _buildingPanel;
        private static GroupBox _cardsContainer;
        private const string BuildingPanelName = "building-panel";
        private const string CardsContainerName = "groupbox";

        private const string CardNamePrefix = "card-";

        private int _tempSelectedBuildID;

        public static BuildingPanelMono Instance { private set; get; }
        public bool IsVisible { private set; get; }

        private const int ShowDuration = 1;
        private const int HideDuration = 1;
        private const float PanelHeight = 333f;
        private const float BottomMargin = 10f;

        public static event Action<Button, int> OnBuildSelected;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _buildingPanel = _root.Q<VisualElement>(BuildingPanelName);
            _cardsContainer = _root.Q<GroupBox>(CardsContainerName);


            if (_buildingPanel != null)
            {
                _buildingPanel.style.display = DisplayStyle.None;
                IsVisible = false;
            }

            if (_buildingCardTemplate == null)
            {
                Debug.LogError("ButtonTemplate not added to script. " + this);
            }

            _tempSelectedBuildID = -1;
            OnBuildSelected += BuildSelected;
        }

        private void CancelBuilding()
        {
            SetButtonEnabled(_tempSelectedBuildID, true);
            _tempSelectedBuildID = -1; // reset temp id
        }

        private void SetButtonEnabled(int id, bool value)
        {
            _root.Query<Button>().AtIndex(id).SetEnabled(value);
        }

        private void BuildSelected(Button button, int index)
        {
            // here + buildingStateSystem
            if (_tempSelectedBuildID < 0) // temp not set
            {
                SetButtonEnabled(index, false);
                _tempSelectedBuildID = index;
            }
            else if (_tempSelectedBuildID != index) // temp != index
            {
                SetButtonEnabled(index, false);
                SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBuildID = index;
            }
            else // temp = index
            {
                Debug.LogWarning("We have a problem with enable/disable buttons in BuildPanelUI." + this);
            }
        }

        #region Show/Hide Panel

        public void SetElementVisible(bool value)
        {
            //   Debug.LogWarning(value + " " + this);
            switch (IsVisible)
            {
                case false when value:
                    Show();
                    IsVisible = true;
                    break;
                case true when !value:
                    Hide();
                    IsVisible = false;
                    break;
            }
        }

        public void Show()
        {
            Debug.LogWarning("show " + this);
            _buildingPanel.style.display = DisplayStyle.Flex;

            _buildingPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = PanelHeight },
                    new StyleValues { bottom = BottomMargin },
                    ShowDuration)
                .Ease(Easing.OutElastic)
                .KeepAlive();
        }

        public void Hide()
        {
            Debug.LogWarning("hide " + this);
            _buildingPanel.experimental.animation
                .Start(
                    new StyleValues { bottom = BottomMargin },
                    new StyleValues { bottom = PanelHeight },
                    HideDuration)
                .Ease(Easing.InQuad)
                .KeepAlive()
                .onAnimationCompleted = () => _buildingPanel.style.display = DisplayStyle.None;
        }

        #endregion

        // TODO cache
        public void InstantiateBuildingsCards(int buildingsCount, NativeList<FixedString32Bytes> names)
        {
            _cardsContainer.Clear();

            for (var i = 0; i < buildingsCount; i++)
            {
                var card = _buildingCardTemplate.Instantiate();
                card.name = CardNamePrefix + i;
                _cardsContainer.Add(card);
                Debug.Log($"card = {i} = {card}");
            }


            var buttons = _cardsContainer.Query<Button>();

            var index = 0;
            buttons.ForEach(element =>
            {
//                Debug.LogWarning(element);
                int index1 = index;
                element.name = index1.ToString(); // TODO
                element.text = names.ElementAt(index1).ToString(); // TODO
                element.RegisterCallback<ClickEvent>(
                    evt => OnBuildSelected?.Invoke(evt.currentTarget as Button, index1));
                ++index;
            });
        }

        public void ClearBuildingsCards()
        {
            _cardsContainer.Clear();
        }
    }
}