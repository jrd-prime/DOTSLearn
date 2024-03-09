using System;
using System.Collections.Generic;
using Sources.Scripts.CommonData;
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

        public bool IsShopSelected { get; private set; }

        private const string BuildingPanelTitle = "blueprints";

        public static BlueprintsCards BlueprintsCards;
        public static event Action<Button, int> OnBlueprintSelected;

        #endregion

        #region Instance/Destroy

        public static BlueprintsShopPanelUI Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnDestroy()
        {
            PanelCloseButton.clicked -= OnCloseButton;
            OnBlueprintSelected -= Blueprint_Selected;
            BlueprintsCards.UnregisterCardsCallbacks(OnBlueprintSelected);
        }

        #endregion


        private void OnEnable()
        {
            PanelIdName = "building-panel";
            PanelTitleIdName = "head-text";

            PanelRoot = GetComponent<UIDocument>().rootVisualElement;
            Panel = PanelRoot.Q<VisualElement>(PanelIdName);
            PanelTitleLabel = Panel.Q<Label>(PanelTitleIdName);
            _cardsContainer = PanelRoot.Q<GroupBox>(Names.CardsContainerIdName);

            PanelCloseButton = Panel.Q<Button>(CloseButtonIdName);


            BlueprintsCards = new BlueprintsCards(_cardsContainer);


            if (Panel == null) return;
            base.HidePanel();
            IsVisible = false;

            PanelCloseButton.clicked += OnCloseButton;
            OnBlueprintSelected += Blueprint_Selected;
        }


        private void SetButtonEnabled(int id, bool value) =>
            _cardsContainer.Query<Button>().AtIndex(id).SetEnabled(value);


        public void InstantiateBuildingsCards(int blueprintsCount, NativeList<FixedString32Bytes> names)
        {
            Assert.IsTrue(blueprintsCount > 0, "Blueprints Count 0, cant init cards for shop");

            BlueprintsCards.InstantiateCards(blueprintsCount, names, OnBlueprintSelected);
        }

        public BlueprintCard GetSelectedCard(int cardId) => BlueprintsCards.GetCardById(cardId);

        public void ShopSelected(int blueprintsCount, NativeList<FixedString32Bytes> blueprintsNames)
        {
            if (!IsShopSelected)
            {
                OpenShop(blueprintsCount, blueprintsNames);
            }
            else
            {
                CloseShop();
            }
        }

        private void CloseShop()
        {
            OnCloseButton();
        }

        private void OpenShop(int blueprintsCount, NativeList<FixedString32Bytes> blueprintsNames)
        {
            InstantiateBuildingsCards(blueprintsCount, blueprintsNames);
            SetPanelTitle(ToString());
            SetElementVisible(true);
            IsShopSelected = true;
        }

        private void Blueprint_Selected(Button button, int index)
        {
            Debug.LogWarning(" = " + this);
            CloseShop();
        }

        protected override void OnCloseButton()
        {
            SetElementVisible(false);
            IsShopSelected = false;
        }
    }
}