using Game.UI.Lobby.Tabs;
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
                if (tab.View != null)
                    builder.RegisterInstance(tab.View).AsSelf();
            }

            // --- Models ---
            builder.Register<TabsModel>(Lifetime.Scoped);

            // --- Controllers ---
            builder.Register<TabsController>(Lifetime.Scoped);
            builder.Register<LobbyController>(Lifetime.Scoped);

            builder.Register<HomeTabController>(Lifetime.Scoped).As<ITabController>();
            builder.Register<MarketTabController>(Lifetime.Scoped).As<ITabController>();
            builder.Register<CollectionTabController>(Lifetime.Scoped).As<ITabController>();

            builder.RegisterEntryPoint<LobbyEntryPoint>();
        }
    }
}
