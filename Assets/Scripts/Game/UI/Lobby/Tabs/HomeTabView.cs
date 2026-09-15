using Game.UI.Lobby.Tabs.Levels;
using UnityEngine;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class HomeTabView : TabView
    {
        [SerializeField] private StartLevelButtonView startLevelButton;
        [SerializeField] private LevelsView levels;

        public override TabType Type => TabType.Home;

        public StartLevelButtonView StartLevelButton => startLevelButton;
        public LevelsView Levels => levels;
    }
}
