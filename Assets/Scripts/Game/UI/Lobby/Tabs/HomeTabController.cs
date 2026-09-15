using Game.UI.Lobby.Tabs.Levels;
using Infrastructure.States;
using Utility.MVC;
using Utility.StateMachine;
using Utility.TabManager;

namespace Game.UI.Lobby.Tabs
{
    public class HomeTabController : Controller<HomeTabView>, ITabController
    {
        private const string LEVEL_LABEL_FORMAT = "Level {0}";

        private readonly LevelsController levelsController;
        private readonly LevelsModel levelsModel;
        private readonly IStateMachine stateMachine;

        public TabType Type => TabType.Home;

        public HomeTabController(HomeTabView view, LevelsController levelsController, LevelsModel levelsModel,
            IStateMachine stateMachine) : base(view)
        {
            this.levelsController = levelsController;
            this.levelsModel = levelsModel;
            this.stateMachine = stateMachine;
        }

        public override void Initialize()
        {
            levelsController.Initialize();

            View.StartLevelButton.SetLabel(string.Format(LEVEL_LABEL_FORMAT, levelsModel.CurrentLevelNumber));
            View.StartLevelButton.Clicked += OnStartLevelClicked;
        }

        public override void Dispose()
        {
            View.StartLevelButton.Clicked -= OnStartLevelClicked;

            levelsController.Dispose();
        }

        private void OnStartLevelClicked() =>
            stateMachine.Enter<GameplayState>();
    }
}
