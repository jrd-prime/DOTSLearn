using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Jrd.UI
{
    public class TextPopUpMono : PanelMono
    {
        private VisualElement _root;
        private Label _textPopUpLabel;

        public static TextPopUpMono Instance { private set; get; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            Panel = _root.Q<VisualElement>("popup-cont");
            _textPopUpLabel = _root.Q<Label>("popup-label");

            if (Panel != null) Panel.style.display = DisplayStyle.None;
        }

        public void SetMessage(string message) => _textPopUpLabel.text = message != "" ? message.ToUpper() : "Msg empty";

        public override async void ShowPanel()
        {
            Panel.style.display = DisplayStyle.Flex;
            await Task.Delay(1000);
            base.SetElementVisible(false);
        }

        protected override void OnCloseButton()
        {
            throw new System.NotImplementedException();
        }

        public void ShowPopUp(string message)
        {
            SetMessage(message);
            base.SetElementVisible(true);
        }
    }
}