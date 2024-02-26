namespace Sources.Scripts.UI
{
    public interface IVisibleElement
    {
        void SetElementVisible(bool value)
        {
            Show();
        }
        void Show();
        void Hide();
    }
}