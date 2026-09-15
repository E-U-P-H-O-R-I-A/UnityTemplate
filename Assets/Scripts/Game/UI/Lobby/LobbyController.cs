using System.Collections.Generic;
using Utility.MVC;
using Utility.TabManager;

namespace Game.UI.Lobby
{
    public class LobbyController : Controller<LobbyView>
    {
        private readonly IReadOnlyList<ITabController> tabControllers;
        private readonly TabsController tabsController;

        public LobbyController(LobbyView view, TabsController tabsController, IReadOnlyList<ITabController> tabControllers)
            : base(view)
        {
            this.tabControllers = tabControllers;
            this.tabsController = tabsController;
        }

        public override void Initialize()
        {
            foreach (ITabController tabController in tabControllers)
                tabController.Initialize();

            tabsController.Initialize();

            View.Show();
        }

        public override void Dispose()
        {
            tabsController.Dispose();

            foreach (ITabController tabController in tabControllers)
                tabController.Dispose();
        }
    }
}
