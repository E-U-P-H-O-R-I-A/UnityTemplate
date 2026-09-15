using Utility.MVC;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class CollectionTabController : Controller<CollectionTabView>, ITabController
    {
        public TabType Type => TabType.Collection;

        public CollectionTabController(CollectionTabView view) : base(view) { }
    }
}
