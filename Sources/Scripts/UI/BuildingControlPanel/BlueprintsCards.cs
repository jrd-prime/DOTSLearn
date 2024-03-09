using System;
using System.Collections.Generic;
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
            ClearCards();

            foreach (var card in _filledCardsContainer)
            {
                _cardsContainer.Add(card);
            }
        }

        private void InstantiateNewCards(int blueprintsCount, NativeList<FixedString32Bytes> names)
        {
            Debug.Log("InstantiateNewCards");
            ClearCards();

            for (var i = 0; i < blueprintsCount; i++)
            {
                var newCard = new BlueprintCard(names[i].ToString(), i);

                _blueprintsCardsList.Add(newCard);

                TemplateContainer filledCard = newCard.GetFilledCard();

                _cardsContainer.Add(filledCard);

                _filledCardsContainer.Add(filledCard);
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
    }
}