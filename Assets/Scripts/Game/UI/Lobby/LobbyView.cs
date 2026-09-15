using UnityEngine;
using UnityEngine.Serialization;
using Utility.MVC;
using Utility.TabManager;

namespace Game.UI.Lobby
{
    public class LobbyView : View
    {
        [FormerlySerializedAs("tabController")]
        [SerializeField] private TabsView tabsView;

        public TabsView Tabs => tabsView;
    }
}
