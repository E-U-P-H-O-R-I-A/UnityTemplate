using Data;
using Services.WindowsService.Windows;
using WindowType = Data.WindowPublicContainer.Id;

namespace Services.WindowsService
{
    public interface IWindowService : IService
    {
        void Initialize();
        
        void OpenWindow(WindowType type, BaseWindowParams @params = null);
        
        void OpenSubWindow(WindowType type, BaseWindowParams @params = null);
    }
}
