namespace Utility.MVC
{
    public abstract class Controller<TView> : IController where TView : class, IView
    {
        protected TView View { get; }

        protected Controller(TView view)
        {
            View = view;
        }

        public virtual void Initialize() { }

        public virtual void Dispose() { }
    }
}
