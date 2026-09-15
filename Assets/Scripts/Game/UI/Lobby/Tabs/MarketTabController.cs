using Utility.MVC;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class MarketTabController : Controller<MarketTabView>, ITabController
    {
        public TabType Type => TabType.Market;

        public MarketTabController(MarketTabView view) : base(view) { }
    }
}
