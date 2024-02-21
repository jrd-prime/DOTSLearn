namespace GamePlay.UI
{
    public interface IVisibleElement
    {
        virtual void SetElementVisible(bool value)
        {
            Show();
        }
        void Show();
        void Hide();
    }
}