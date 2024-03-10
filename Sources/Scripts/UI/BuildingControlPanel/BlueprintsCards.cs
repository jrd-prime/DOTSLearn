using System;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.UI.BuildingControlPanel
{
    public readonly struct BlueprintsCards
    {
        private readonly GroupBox _cardsContainer;
        private static List<BlueprintCard> _blueprintsCardsList;
        private static List<TemplateContainer> _filledCardsContainer;
        private static bool _isCallbacksRegistered;

        public BlueprintsCards(GroupBox cardsContainer)
        {
            _cardsContainer = cardsContainer;
            _blueprintsCardsList = new List<BlueprintCard>(0);
            _filledCardsContainer = new List<TemplateContainer>(0);
        }

        public void InstantiateCards(int blueprintsCount, NativeList<FixedString32Bytes> names,
            Action<Button, int> onBlueprintSelectedCallback)
        {
            ClearCards();

            if (blueprintsCount != _blueprintsCardsList.Count)
            {
                InstantiateNewCards(blueprintsCount, names);
            }
            else
            {
                LoadCachedCards();
            }

            if (!_isCallbacksRegistered) RegisterCardsCallbacks(onBlueprintSelectedCallback);

            names.Dispose();
        }

        private void LoadCachedCards()
        {
            Debug.Log("LoadCachedCards");

            foreach (var card in _filledCardsContainer)
            {
                _cardsContainer.Add(card);
            }
        }

        private void InstantiateNewCards(int blueprintsCount, NativeList<FixedString32Bytes> names)
        {
            Debug.Log("InstantiateNewCards");

            for (var i = 0; i < blueprintsCount; i++)
            {
                var newCard = new BlueprintCard(names[i].ToString(), i);

                TemplateContainer filledCard = newCard.GetFilledCard();

                _cardsContainer.Add(filledCard);
                _filledCardsContainer.Add(filledCard);
                _blueprintsCardsList.Add(newCard);
            }
        }

        public void ClearCards() => _cardsContainer.Clear();
        public List<BlueprintCard> GetCardsList() => _blueprintsCardsList;

        #region Callbacks

        private static void RegisterCardsCallbacks(Action<Button, int> onBlueprintSelectedCallback)
        {
            foreach (var card in _blueprintsCardsList)
            {
                card.Button.RegisterCallback<ClickEvent>(
                    evt => onBlueprintSelectedCallback?.Invoke(evt.currentTarget as Button, card.Id));
            }

            _isCallbacksRegistered = true;
        }

        public static void UnregisterCardsCallbacks(Action<Button, int> onBlueprintSelectedCallback)
        {
            if (!_isCallbacksRegistered) return;

            foreach (var card in _blueprintsCardsList)
            {
                card.Button.UnregisterCallback<ClickEvent>(
                    evt => onBlueprintSelectedCallback?.Invoke(evt.currentTarget as Button, card.Id));
            }

            _isCallbacksRegistered = false;
        }

        #endregion

        public BlueprintCard GetCardById(int cardId)
        {
            Assert.IsTrue(_blueprintsCardsList.Count > 0, $"_blueprintsCardsList.Count > 0 {this}");

            return _blueprintsCardsList[cardId];
        }
    }
}