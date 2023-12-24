using UnityEngine;
using UnityEngine.UIElements;

namespace Jrd.JUI
{
    public class MainMenuUI : MonoBehaviour
    {
        public static MainMenuUI Instance;
        private VisualElement _root;

        private MainMenuUI()
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
            _root = GetComponent<UIDocument>().rootVisualElement;

            var menuPanel = _root.Q<VisualElement>("menu-panel");
            var returnButton = _root.Q<Button>("menu-return-button");
            var restartButton = _root.Q<Button>("menu-restart-button");
            var exitButton = _root.Q<Button>("menu-exit-button");


            HideElemenet(_root); // TODO 

            returnButton.clicked += () => HideElemenet(_root); // TODO Return
            restartButton.clicked += () => Debug.Log("restart"); // TODO restart
            exitButton.clicked += Application.Quit; // TODO
        }

        public void HideMenu()
        {
            if (_root.style.display == DisplayStyle.None)
            {
                GetComponent<UIDocument>().sortingOrder = 100;
                ShowElemenet(_root);
                // root.experimental.animation.Layout(new Rect(Vector2.zero, root.layout.size), 1500).Ease(Easing.OutCirc);
            }
            else
            {
                GetComponent<UIDocument>().sortingOrder = 0;
                HideElemenet(_root);
                // root.experimental.animation.Layout(new Rect(new Vector2(-root.layout.width, 0), root.layout.size), 1500)
                //     .Ease(Easing.OutCirc);
            }
        }

        private void HideElemenet(VisualElement e)
        {
            e.style.display = DisplayStyle.None;
        }
        private void ShowElemenet(VisualElement e)
        {
            e.style.display = DisplayStyle.Flex;
        }
    }
}