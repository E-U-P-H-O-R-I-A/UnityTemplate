namespace Utility.MVC
{
    public interface IView
    {
        bool IsVisible { get; }

        void Show();

        void Hide();
    }
}
