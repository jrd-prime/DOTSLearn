using System;
using System.Collections.Generic;
using GamePlay.Shop.BlueprintsShop;
using GamePlay.UI.BuildingControlPanel;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.UI.BlueprintsShopPanel
{
    public class BlueprintsShopPanelUI : PanelMono
    {
        private static GroupBox _cardsContainer;
        private static List<BuildingCard> _cards;

        private const string CardsContainerName = "groupbox";
        private const string BuildingPanelTitle = "blueprints";

        private const int ShowDuration = 1;
        private const int HideDuration = 1;
        private const float PanelHeight = 333f;
        private const float BottomMargin = 10f;

        public static BlueprintsShopPanelUI Instance { private set; get; }
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
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = Panel.Q<Label>(PanelTitleIdName);
            _cardsContainer = PanelRoot.Q<GroupBox>(CardsContainerName);

            CloseButtonId = BCPNamesID.CloseButtonId;
            PanelCloseButton = Panel.Q<Button>(CloseButtonId);

            if (Panel == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }

        private void OnDestroy()
        {
            PanelCloseButton.clicked -= OnCloseButton;
        }

        private void SetButtonEnabled(int id, bool value) =>
            _cardsContainer.Query<Button>().AtIndex(id).SetEnabled(value);

        public static BuildingCard GetSelectedCard(int cardId) => _cards[cardId];
        public void ClearBuildingsCards() => _cardsContainer.Clear();

        // TODO cache
        public void InstantiateBuildingsCards(int blueprintsCount, NativeList<FixedString32Bytes> names)
        {
            Debug.Log(blueprintsCount);
            Debug.Log(names[0]);
            Debug.Log("init cards");
            ClearBuildingsCards();

            _cards = new List<BuildingCard>(blueprintsCount);
            for (var i = 0; i < blueprintsCount; i++)
            {
                var card = new BuildingCard(names[i].ToString(), i);
                _cards.Add(card);
                _cardsContainer.Add(card.GetFilledCard());
                card.Button.RegisterCallback<ClickEvent>(
                    evt => OnBuildSelected?.Invoke(evt.currentTarget as Button, card.Id));
            }

            names.Dispose();
        }

        protected override void OnCloseButton() => SetElementVisible(false);
    }
}