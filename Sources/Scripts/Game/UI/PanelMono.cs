using UnityEngine;
using UnityEngine.UIElements;

namespace GamePlay.UI
{
    public abstract class PanelMono : MonoBehaviour
    {
        protected int PanelTitleMaxLength = 30;
        protected bool IsVisible;
        protected VisualElement PanelRoot;
        protected VisualElement Panel;
        protected Label PanelTitleLabel;

        public Button PanelCloseButton;

        protected string PanelIdName;
        protected string PanelTitleIdName;
        protected string CloseButtonId;


        public bool IsPanelVisible
        {
            get => IsVisible;
            protected set => IsVisible = value;
        }

        protected abstract void OnCloseButton();

        public virtual void SetElementVisible(bool value)
        {
            switch (IsVisible)
            {
                case false when value:
                    ShowPanel();
                    IsVisible = true;
                    break;
                case true when !value:
                    HidePanel();
                    IsVisible = false;
                    break;
            }
        }

        public virtual void ShowPanel()
        {
            Panel.style.display = DisplayStyle.Flex;
        }

        public virtual void HidePanel()
        {
            Panel.style.display = DisplayStyle.None;
        }

        public virtual void SetPanelTitle(string panelTitle)
        {
            if (PanelTitleLabel != null && (panelTitle != "" || panelTitle.Length > PanelTitleMaxLength))
            {
                PanelTitleLabel.text = panelTitle.ToUpper();
            }
        }
    }
}