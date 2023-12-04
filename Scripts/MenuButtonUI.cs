using System.Collections;
using System.Collections.Generic;
using Jrd.JUI;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd
{
    public class MenuButtonUI : MonoBehaviour
    {
        public static MenuButtonUI Instance;
        private MainMenuUI a;

        private MenuButtonUI()
        {
        }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var menuBtn = root.Q<Button>("menuBtn");
            var focusController = root.focusController;

            menuBtn.clicked += OnMenuButtonClicked;
        }

        private static void OnMenuButtonClicked()
        {
            MainMenuUI.Instance.HideMenu();
        }
    }
}