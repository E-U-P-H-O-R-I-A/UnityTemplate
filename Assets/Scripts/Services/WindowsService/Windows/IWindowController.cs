using System;
using Cysharp.Threading.Tasks;
using Utility.MVC;

namespace Services.WindowsService.Windows
{
    public interface IWindowController : IController
    {
        event Action<IWindowController> Closed;

        bool IsVisible { get; }

        UniTask Open(WindowParams windowParams);

        UniTask Close();

        void ForceShow();

        void ForceHide();

        void BringToFront();
    }
}
