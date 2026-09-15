using Utility.MVC;

namespace Utility.TabManager
{
    public abstract class TabView : View
    {
        public abstract TabType Type { get; }
    }
}
