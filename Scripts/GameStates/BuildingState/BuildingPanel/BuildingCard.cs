using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.GameStates.BuildingState.BuildingPanel
{
    public struct BuildingCard
    {
        private readonly TemplateContainer _card;
        private const string CardTemplatePath = "UXMLTemplates/BuildingPanel/card";

        public string Name { get; }
        public int Id { get; }
        public Label Title { get; }
        public VisualElement Image { get; }
        public Button Button { get; }

        // Prefix
        private const string CardNamePrefix = "card-";
        private const string CardButtonNamePrefix = "building-";

        // Card
        private const string CardHeadName = "head-text";
        private const string CardImageName = "img";
        private const string CardButtonName = "btn";

        public BuildingCard(string buildingName, int cardIndex)
        {
            Name = buildingName;
            Id = cardIndex;

            VisualTreeAsset cardTemplate = Resources.Load(CardTemplatePath, typeof(VisualTreeAsset)) as VisualTreeAsset;

            if (cardTemplate == null)
            {
                throw new FileNotFoundException("Card template not found.", CardTemplatePath);
            }

            _card = cardTemplate.Instantiate();
            Title = _card.Q<Label>(CardHeadName);
            Image = _card.Q<VisualElement>(CardImageName);
            Button = _card.Q<Button>(CardButtonName);
        }

        public TemplateContainer GetFilledCard()
        {
            _card.name = CardNamePrefix + Id;

            // Head
            Title.text = Name;
            // Icon
            Image.style.backgroundColor = new StyleColor(Color.green);
            // Button
            Button.text = Name;
            Button.name = CardButtonNamePrefix + Id;

            return _card;
        }
    }
}