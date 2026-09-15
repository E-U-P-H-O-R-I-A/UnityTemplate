using UnityEngine;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class HomeTabView : TabView
    {
        [SerializeField] private StartLevelButtonView startLevelButton;

        public override TabType Type => TabType.Home;

        public StartLevelButtonView StartLevelButton => startLevelButton;
    }
}
