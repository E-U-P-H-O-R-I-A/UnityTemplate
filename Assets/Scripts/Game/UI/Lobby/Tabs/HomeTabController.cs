using Infrastructure.States;
using Utility.MVC;
using Utility.StateMachine;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class HomeTabController : Controller<HomeTabView>, ITabController
    {
        private readonly IStateMachine stateMachine;

        public TabType Type => TabType.Home;

        public HomeTabController(HomeTabView view, IStateMachine stateMachine) : base(view)
        {
            this.stateMachine = stateMachine;
        }

        public override void Initialize() =>
            View.StartLevelButton.Clicked += OnStartLevelClicked;

        public override void Dispose() =>
            View.StartLevelButton.Clicked -= OnStartLevelClicked;

        private void OnStartLevelClicked() =>
            stateMachine.Enter<GameplayState>();
    }
}
