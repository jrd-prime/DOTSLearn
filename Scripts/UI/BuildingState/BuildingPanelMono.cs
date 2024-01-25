using System;
using Unity.Collections;
using UnityEngine;
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
        private int _tempSelectedBuildID;

        public static BuildingPanelMono Instance { private set; get; }
        public bool IsVisible { private set; get; }

        private const string BuildingPanelName = "building-panel";
        private const string BuildingPanelTitleName = "head-text";
        private const string CardsContainerName = "groupbox";
        private const string CardNamePrefix = "card-";
        private const string CardButtonNamePrefix = "building-";
        private const string BuildingPanelTitle = "blueprints";

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
            _cardsContainer.Query<Button>().AtIndex(id).SetEnabled(value);
        }

        private void BuildSelected(Button button, int index)
        {
            // temp not set
            if (_tempSelectedBuildID < 0)
            {
                SetButtonEnabled(index, false);
                _tempSelectedBuildID = index;
            }
            // temp != index
            else if (_tempSelectedBuildID != index)
            {
                SetButtonEnabled(index, false);
                SetButtonEnabled(_tempSelectedBuildID, true);
                _tempSelectedBuildID = index;
            }
            // temp = index
            else
            {
                Debug.LogWarning("We have a problem with enable/disable buttons in BuildPanelUI." + this);
            }
        }

        #region Show/Hide Panel

        public void SetElementVisible(bool value)
        {
            switch (IsVisible)
            {
                case false when value:
                    SetPanelTitle(BuildingPanelTitle);
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

        #region Set Data / Instantiate

        private void SetPanelTitle(string titleText)
        {
            _buildingPanel.Q<Label>(BuildingPanelTitleName).text = titleText.ToUpper();
        }

        // TODO cache
        public void InstantiateBuildingsCards(int buildingsCount, NativeList<FixedString32Bytes> names)
        {
            _cardsContainer.Clear();

            for (var i = 0; i < buildingsCount; i++)
            {
                var filledCard = GetCardWithData(names[i].ToString(), i);
                _cardsContainer.Add(filledCard);
            }
        }

        private TemplateContainer GetCardWithData(string buildingName, int buildingIndex)
        {
            TemplateContainer newCard = _buildingCardTemplate.Instantiate();

            var cardHead = newCard.Q<Label>("head-text");
            var cardImage = newCard.Q<VisualElement>("img");
            var cardButton = newCard.Q<Button>("btn");

            newCard.name = CardNamePrefix + buildingIndex;

            // Head
            cardHead.text = $"{buildingIndex} {buildingName}";

            // Icon
            cardImage.style.backgroundColor = new StyleColor(Color.green);

            // Button
            cardButton.text = buildingName;
            cardButton.name = CardButtonNamePrefix + buildingIndex;
            cardButton.RegisterCallback<ClickEvent>(
                evt => OnBuildSelected?.Invoke(evt.currentTarget as Button, buildingIndex));

            Debug.Log($"card = {buildingIndex} = {newCard}");

            return newCard;
        }

        public void ClearBuildingsCards() => _cardsContainer.Clear();

        #endregion
    }
}