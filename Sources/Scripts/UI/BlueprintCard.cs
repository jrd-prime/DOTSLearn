using Sources.Scripts.CommonData;
using Sources.Scripts.UI.BuildingControlPanel;
using Sources.Scripts.Utility;
using Unity.Assertions;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sources.Scripts.UI
{
    public readonly struct BlueprintCard
    {
        public TemplateContainer Card { get; }
        public string Name { get; }
        public int Id { get; }
        public Label Title { get; }
        public Button Button { get; }

        private readonly VisualElement _image;

        public BlueprintCard(string buildingName, int cardIndex)
        {
            Name = buildingName;
            Id = cardIndex;

            var cardTemplate = Resources.Load(ResPath.CardTemplatePath, typeof(VisualTreeAsset)) as VisualTreeAsset;

            Assert.IsNotNull(cardTemplate, $"Card template not found. Path: {ResPath.CardTemplatePath}");

            Card = cardTemplate!.Instantiate();
            Title = Card.Q<Label>(Names.CardHeadName);
            _image = Card.Q<VisualElement>(Names.CardImageName);
            Button = Card.Q<Button>(Names.CardButtonName);
        }

        public TemplateContainer GetFilledCard()
        {
            SetNameId();
            SetTitle(Name);
            SetImage();
            SetButtonText(Name);
            SetButtonNameId();

            return Card;
        }

        private void SetButtonNameId() => Button.name = BCPNamesID.CardButtonNamePrefix + Id;

        public void SetButtonText(string name)
        {
            Assert.IsTrue(TextUtils.IsStringCompliant(name, NameLength.ShopCardButtonTextMaxLength));

            Button.text = TextUtils.IsStringCompliant(name, NameLength.ShopCardTitleMaxLength)
                ? name
                : "no text".ToUpper();
        }

        private void SetImage()
        {
            string path = ResPath.BuildingsPreviewPath + Name.ToLower();
            Sprite sprite = Resources.Load<Sprite>(path);

            if (sprite == null)
            {
                path = ResPath.BuildingsPreviewPath + ResPath.BuildingsPreviewNoImageName;
                sprite = Resources.Load<Sprite>(path);
            }

            Assert.IsNotNull(sprite, $"Card image sprite is null. Path: {path}");

            _image.style.backgroundImage = new StyleBackground(sprite);
        }

        public void SetTitle(string name)
        {
            Assert.IsTrue(TextUtils.IsStringCompliant(name, NameLength.ShopCardTitleMaxLength));

            Title.name = TextUtils.IsStringCompliant(name, NameLength.ShopCardTitleMaxLength)
                ? name
                : "no text".ToUpper();
        }

        private void SetNameId() => Card.name = BCPNamesID.CardNamePrefix + Id;
    }
}