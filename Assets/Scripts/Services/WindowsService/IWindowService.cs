using System;
using Services.WindowsService.Windows;
using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService
{
    public interface IWindowService : IInitializableService
    {
        event Action<WindowType> WindowClosed;

        void OpenWindow(WindowType type, WindowParams windowParams = null);

        void OpenSubWindow(WindowType type, WindowParams windowParams = null);
    }
}
