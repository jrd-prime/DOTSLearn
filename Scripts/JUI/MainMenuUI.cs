using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

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
            _root.style.display = DisplayStyle.None; // TODO 
            var returnButton = _root.Q<Button>("menu-return-button");
            var restartButton = _root.Q<Button>("menu-restart-button");
            var exitButton = _root.Q<Button>("menu-exit-button");

            returnButton.clicked += () =>
            {
                _root.style.display = DisplayStyle.None;
                Debug.Log("return");
            };
            restartButton.clicked += () =>
            {
                SceneManager.LoadScene("SampleScene");
                Debug.Log("restart");
            };
            exitButton.clicked += () =>
            {
                Debug.Log("exit");
                Application.Quit();
            };
        }

        public void HideMenu()
        {
            // var display = _root.style.display == DisplayStyle.None;
            // _root.style.display = (display) ? DisplayStyle.Flex : DisplayStyle.None;
            if (_root.style.display == DisplayStyle.None)
            {
                GetComponent<UIDocument>().sortingOrder = 100;
                _root.style.display = DisplayStyle.Flex;
                // root.experimental.animation.Layout(new Rect(Vector2.zero, root.layout.size), 1500).Ease(Easing.OutCirc);
            }
            else
            {
                GetComponent<UIDocument>().sortingOrder = 0;
                _root.style.display = DisplayStyle.None;
                // root.experimental.animation.Layout(new Rect(new Vector2(-root.layout.width, 0), root.layout.size), 1500)
                //     .Ease(Easing.OutCirc);
            }
        }
    }
}

//
// var root = GetComponent<UIDocument>().rootVisualElement;
//             m_MainMenuPanel = root.Q<VisualElement>("main-menu-container");
//             m_NameField = root.Q<TextField>("name-field");
//             m_NameField.value = "Player";
// #if (!UNITY_IPHONE || !UNITY_ANDROID)
//             if (!string.IsNullOrEmpty(Environment.UserName))
//                 m_NameField.value = Environment.UserName.ToUpper();
// #endif
//
//             m_IpField = root.Q<TextField>("ip-field");
//             m_PortField = root.Q<TextField>("port-field");
//
//             m_JoinButton = root.Q<Button>("main-menu-start-button");
//             m_JoinButton.clicked += OnJoinButtonClicked;
//
//             m_FocusController = root.focusController;
//             
//             
//             
//             private void Start()
//                     {
//                         // If we are running a Client, we show the IP and Port to connect
//                         if (ClientServerBootstrap.RequestedPlayType == ClientServerBootstrap.PlayType.Client)
//                         {
//                             m_JoinButton.text = "JOIN";
//                             // Set the focus ring manually
//                             m_FocusRing = new VisualElement[] {m_NameField, m_IpField, m_PortField, m_JoinButton};
//                         }
//                         else
//                         {
//                             m_JoinButton.text = "START CLIENT & SERVER";
//                             m_IpField.style.display = DisplayStyle.None;
//                             m_PortField.style.display = DisplayStyle.None;
//             
//                             // Set the focus ring manually
//                             m_FocusRing = new VisualElement[] {m_NameField, m_JoinButton};
//                         }
//             
//             
//                         // Set focus to the name text field
//                         m_NameField.Focus();
//                         StartCoroutine(UpdateInput());
//                     }
//             
//                     private void OnDisable()
//                     {
//                         m_JoinButton.clicked -= OnJoinButtonClicked;
//                         StopCoroutine(UpdateInput());
//                     }
//             
//                     private void OnJoinButtonClicked()
//                     {
//                         if (!ServerConnectionUtils.ValidateIPv4(m_IpField.value))
//                         {
//                             Popup.Instance.Show("Error", "Please enter a valid IP.", "Retry");
//                             return;
//                         }
//             
//                         // Assign Player Name
//                         PlayerInfoController.Instance.LocalPlayerName = m_NameField.value;
//             
//                         // Disable Main Menu
//                         m_MainMenuPanel.style.display = DisplayStyle.None;
//             
//                         // Switch camera
//                         if (MainMenuCameraSwitcher.Instance != null)
//                         {
//                             MainMenuCameraSwitcher.Instance.ShowCarSelectionCamera();
//                             CarSelectionUI.Instance.ShowCarSelection(true);
//                         }
//             
//                         // Stop checking input in Main Menu
//                         StopCoroutine(UpdateInput());
//                         m_InMainMenu = false;
//             
//                         // Set Player Info for Connection
//                         PlayerInfoController.Instance.SetConnectionInfo(m_IpField.value, m_PortField.value);
//                         PlayerAudioManager.Instance.PlayClick();
//                     }
//             
//             
//                     private IEnumerator UpdateInput()
//                     {
//                         while (m_InMainMenu)
//                         {
//                             // TODO: Check Navigation when Unity Input System is compatible with DOTS
//                             var vertical = Input.GetAxis("Vertical");
//                             vertical -= Input.GetAxis("LeftStickY"); // Inverted axis
//                             vertical += Input.GetAxis("DPadY");
//                             vertical = math.clamp(vertical, -1, 1);
//             
//                             // Threshold level for Sticks
//                             if (math.abs(vertical) > 0.2f)
//                             {
//                                 switch (vertical)
//                                 {
//                                     case < 0f:
//                                         m_FocusRingIndex = (m_FocusRingIndex + 1) % m_FocusRing.Length;
//                                         FocusElement(m_FocusRingIndex);
//                                         break;
//                                     case > 0f:
//                                         m_FocusRingIndex = math.abs((m_FocusRingIndex - 1) % m_FocusRing.Length);
//                                         FocusElement(m_FocusRingIndex);
//                                         break;
//                                 }
//             
//                                 yield return new WaitForSeconds(0.25f);
//                             }
//             
//                             yield return null;
//                         }
//             
//                         yield return null;
//                     }
//             
//                     private void FocusElement(int index)
//                     {
//                         if (m_FocusController.focusedElement != m_FocusRing[index])
//                             m_FocusRing[index].Focus();
//                     }
//                 }