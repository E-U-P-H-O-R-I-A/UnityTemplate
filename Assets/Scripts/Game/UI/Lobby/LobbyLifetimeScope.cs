using Game.UI.Lobby.Tabs;
using Game.UI.Lobby.Tabs.Levels;
using Infrastructure;
using UnityEngine;
using Utility.TabManager;
using VContainer;
using VContainer.Unity;

namespace Game.UI.Lobby
{
    public class LobbyLifetimeScope : SceneLifetimeScope
    {
        [Space]
        [SerializeField] private LobbyView lobby;

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Views ---
            builder.RegisterComponent(lobby);
            builder.RegisterComponent(lobby.Tabs);

            foreach (TabData tab in lobby.Tabs.Tabs)
            {
                if (tab.View == null)
                    continue;

                builder.RegisterInstance(tab.View).AsSelf();

                if (tab.View is HomeTabView homeTab && homeTab.Levels != null)
                    builder.RegisterComponent(homeTab.Levels);
            }

            // --- Models ---
            builder.Register<TabsModel>(Lifetime.Scoped);
            builder.Register<LevelsModel>(Lifetime.Scoped);

            // --- Controllers ---
            builder.Register<TabsController>(Lifetime.Scoped);
            builder.Register<LobbyController>(Lifetime.Scoped);
            builder.Register<LevelsController>(Lifetime.Scoped);

            builder.Register<HomeTabController>(Lifetime.Scoped).As<ITabController>();
            builder.Register<MarketTabController>(Lifetime.Scoped).As<ITabController>();
            builder.Register<CollectionTabController>(Lifetime.Scoped).As<ITabController>();

            builder.RegisterEntryPoint<LobbyEntryPoint>();
        }
    }
}
