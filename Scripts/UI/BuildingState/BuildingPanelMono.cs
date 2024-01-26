using System;
using System.Collections.Generic;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

namespace Jrd
{
    public class BuildingPanelMono : MonoBehaviour, IVisibleElement
    {
        private VisualElement _root;
        private VisualElement _buildingPanel;
        private static GroupBox _cardsContainer;
        private static List<BuildingCard> _cards;

        public static BuildingPanelMono Instance { private set; get; }
        public bool IsVisible { private set; get; }

        private const string BuildingPanelName = "building-panel";
        private const string BuildingPanelTitleName = "head-text";
        private const string CardsContainerName = "groupbox";
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
        }

        private void SetButtonEnabled(int id, bool value)
        {
            _cardsContainer.Query<Button>().AtIndex(id).SetEnabled(value);
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

        public static BuildingCard GetSelectedCard(int cardId)
        {
            return _cards[cardId];
        }

        #region Set Data / Instantiate

        private void SetPanelTitle(string titleText)
        {
            _buildingPanel.Q<Label>(BuildingPanelTitleName).text = titleText.ToUpper();
        }

        // TODO cache
        public void InstantiateBuildingsCards(int buildingsCount, NativeList<FixedString32Bytes> names)
        {
            _cardsContainer.Clear();

            _cards = new List<BuildingCard>(buildingsCount);

            for (var i = 0; i < buildingsCount; i++)
            {
                var card = new BuildingCard(names[i].ToString(), i);

                _cards.Add(card);
                _cardsContainer.Add(card.GetFilledCard());

                card.Button.RegisterCallback<ClickEvent>(
                    evt => OnBuildSelected?.Invoke(evt.currentTarget as Button, card.Id));
            }
        }

        public void ClearBuildingsCards() => _cardsContainer.Clear();

        #endregion
    }
}