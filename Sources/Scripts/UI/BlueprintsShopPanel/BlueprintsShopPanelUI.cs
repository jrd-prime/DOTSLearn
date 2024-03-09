using System;
using System.Collections.Generic;
using Sources.Scripts.UI.BuildingControlPanel;
using Unity.Assertions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.UI.BlueprintsShopPanel
{
    public class BlueprintsShopPanelUI : PanelMono
    {
        #region Private

        private static GroupBox _cardsContainer;
        private static List<BlueprintCard> _cards;

        private static List<BlueprintCard> _cardsListCache;

        private const string CardsContainerName = "groupbox";
        private const string BuildingPanelTitle = "blueprints";

        private const int ShowDuration = 1;
        private const int HideDuration = 1;
        private const float PanelHeight = 333f;
        private const float BottomMargin = 10f;

        public static BlueprintsCards BlueprintsCards;
        public static event Action<Button, int> OnBlueprintSelected;

        #endregion

        public static BlueprintsShopPanelUI Instance { private set; get; }


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


            BlueprintsCards = new BlueprintsCards(_cardsContainer);


            if (Panel == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
        }

        private void OnDestroy()
        {
            PanelCloseButton.clicked -= OnCloseButton;
            BlueprintsCards.UnregisterCardsCallbacks(OnBlueprintSelected);
        }

        private void SetButtonEnabled(int id, bool value) =>
            _cardsContainer.Query<Button>().AtIndex(id).SetEnabled(value);

        public BlueprintCard GetSelectedCard(int cardId) => _cards[cardId];

        public void InstantiateBuildingsCards(int blueprintsCount, NativeList<FixedString32Bytes> names)
        {
            Assert.IsTrue(blueprintsCount > 0, "Blueprints Count 0, cant init cards for shop");

            BlueprintsCards.InstantiateCards(blueprintsCount, names, OnBlueprintSelected);
        }


        protected override void OnCloseButton()
        {
            SetElementVisible(false);
        }
    }
}