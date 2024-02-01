using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd
{
    public abstract class PanelMono : MonoBehaviour
    {
        protected int PanelTitleMaxLength = 30;
        protected bool IsVisible;
        protected VisualElement PanelRoot;
        protected VisualElement PanelVisualElement;
        protected Label PanelTitleLabel;

        public Button PanelCloseButton;

        protected string PanelIdName;
        protected string PanelTitleIdName;
        protected string PanelCloseButtonIdName;

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
                    Debug.Log("set visible");
                    ShowPanel();
                    IsVisible = true;
                    break;
                case true when !value:
                    Debug.Log("set in visible");
                    HidePanel();
                    IsVisible = false;
                    break;
            }
        }

        public virtual void ShowPanel()
        {
            PanelVisualElement.style.display = DisplayStyle.Flex;
        }

        public virtual void HidePanel()
        {
            PanelVisualElement.style.display = DisplayStyle.None;
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