using Game.UI.Lobby.HomeTab;
using UnityEngine;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class HomeTab : Tab
    {
        [SerializeField] private StartLevelButton startLevelButton;
        
        public override TabType Type => TabType.Home;

        public override void Initialize()
        {
            base.Initialize();
            
            startLevelButton.Initialize();
        }
    }
}