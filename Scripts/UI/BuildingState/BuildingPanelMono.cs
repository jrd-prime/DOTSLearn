using System;
using System.Collections.Generic;
using Jrd.GameStates.BuildingState.BuildingPanel;
using Unity.Collections;
using UnityEngine.UIElements;

namespace Jrd
{
    public class BuildingPanelMono : PanelMono
    {
        private static GroupBox _cardsContainer;
        private static List<BuildingCard> _cards;
        
        private const string CardsContainerName = "groupbox";
        private const string BuildingPanelTitle = "blueprints";

        private const int ShowDuration = 1;
        private const int HideDuration = 1;
        private const float PanelHeight = 333f;
        private const float BottomMargin = 10f;

        public static BuildingPanelMono Instance { private set; get; }
        public static event Action<Button, int> OnBuildSelected;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            PanelIdName = "building-panel";
            PanelTitleIdName = "head-text";
            
            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            BuildingPanel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = BuildingPanel.Q<Label>(PanelTitleIdName);
            _cardsContainer = PanelRoot.Q<GroupBox>(CardsContainerName);

            if (BuildingPanel == null) return;
            base.HidePanel();
            IsVisible = false;
        }

        private void SetButtonEnabled(int id, bool value) =>
            _cardsContainer.Query<Button>().AtIndex(id).SetEnabled(value);
        
        public static BuildingCard GetSelectedCard(int cardId) => _cards[cardId];
        public void ClearBuildingsCards() => _cardsContainer.Clear();
        
        // TODO cache
        public void InstantiateBuildingsCards(int buildingsCount, NativeList<FixedString32Bytes> names)
        {
            ClearBuildingsCards();

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

        protected override void OnCloseButton()
        {
            throw new NotImplementedException();
        }
    }
}