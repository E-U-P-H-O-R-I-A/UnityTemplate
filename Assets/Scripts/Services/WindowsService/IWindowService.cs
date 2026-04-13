using Data.Scheme.Public;
using Services.WindowsService.Windows;

namespace Services.WindowsService
{
    public interface IWindowService : IService
    {
        void Initialize();
        
        void OpenWindow(WindowType type, BaseWindowParams @params = null);
        
        void OpenSubWindow(WindowType type, BaseWindowParams @params = null);
    }
}
