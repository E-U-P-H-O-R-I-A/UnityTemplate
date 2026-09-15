using System;
using Services.WindowsService.Windows;
using VContainer;
using Object = UnityEngine.Object;
using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService.Factory
{
    public sealed class Window : IDisposable
    {
        private readonly IScopedObjectResolver scope;
        private readonly WindowView view;

        public WindowType Type { get; }
        public IWindowController Controller { get; }

        public Window(WindowType type, WindowView view, IWindowController controller, IScopedObjectResolver scope)
        {
            this.scope = scope;
            this.view = view;

            Type = type;
            Controller = controller;
        }

        public void Dispose()
        {
            scope.Dispose();

            if (view != null)
                Object.Destroy(view.gameObject);
        }
    }
}
