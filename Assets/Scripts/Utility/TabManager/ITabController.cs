using Utility.MVC;

namespace Utility.TabManager
{
    public interface ITabController : IController
    {
        TabType Type { get; }
    }
}
