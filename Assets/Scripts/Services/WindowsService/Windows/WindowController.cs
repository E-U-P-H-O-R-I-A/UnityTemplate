using System;
using Cysharp.Threading.Tasks;
using Utility.MVC;

namespace Services.WindowsService.Windows
{
    public abstract class WindowController<TView> : Controller<TView>, IWindowController
        where TView : WindowView
    {
        private bool isProcessingOpening;
        private bool isProcessingClosing;

        public event Action<IWindowController> Closed;

        public bool IsVisible => View.IsVisible;

        protected WindowController(TView view) : base(view) { }

        public override void Initialize() =>
            View.CloseRequested += OnCloseRequested;

        public override void Dispose() =>
            View.CloseRequested -= OnCloseRequested;

        public async UniTask Open(WindowParams windowParams)
        {
            if (isProcessingOpening || isProcessingClosing)
                return;

            isProcessingOpening = true;

            try
            {
                View.SetCloseInteractable(false);

                await OnBeforeOpen(windowParams);
                await View.PlayOpenAsync();
                await OnAfterOpened(windowParams);

                View.SetCloseInteractable(true);
            }
            finally
            {
                isProcessingOpening = false;
            }
        }

        public async UniTask Close()
        {
            if (isProcessingOpening || isProcessingClosing)
                return;

            isProcessingClosing = true;

            try
            {
                View.SetCloseInteractable(false);

                await OnBeforeClose();
                await View.PlayCloseAsync();
                await OnAfterClosed();
            }
            finally
            {
                isProcessingClosing = false;
            }

            Closed?.Invoke(this);
        }

        public void ForceShow() =>
            View.Show();

        public void ForceHide() =>
            View.Hide();

        public void BringToFront() =>
            View.BringToFront();

        protected virtual UniTask OnBeforeOpen(WindowParams windowParams) => UniTask.CompletedTask;

        protected virtual UniTask OnAfterOpened(WindowParams windowParams) => UniTask.CompletedTask;

        protected virtual UniTask OnBeforeClose() => UniTask.CompletedTask;

        protected virtual UniTask OnAfterClosed() => UniTask.CompletedTask;

        private void OnCloseRequested() =>
            Close().Forget();
    }

    public abstract class WindowController<TView, TParams> : WindowController<TView>
        where TView : WindowView
        where TParams : WindowParams, new()
    {
        protected TParams Params { get; private set; }

        protected WindowController(TView view) : base(view) { }

        protected sealed override UniTask OnBeforeOpen(WindowParams windowParams)
        {
            Params = windowParams as TParams ?? new TParams();

            return OnBeforeOpen(Params);
        }

        protected sealed override UniTask OnAfterOpened(WindowParams windowParams) =>
            OnAfterOpened(Params);

        protected virtual UniTask OnBeforeOpen(TParams windowParams) => UniTask.CompletedTask;

        protected virtual UniTask OnAfterOpened(TParams windowParams) => UniTask.CompletedTask;
    }
}
