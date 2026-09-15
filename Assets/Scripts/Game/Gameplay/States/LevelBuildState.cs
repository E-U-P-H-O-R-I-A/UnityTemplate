using Cysharp.Threading.Tasks;
using Data;
using Game.UI.Lobby.Tabs.Levels;
using Services.InputService;
using Services.LevelBuilder;
using Services.LogService;
using Utility.StateMachine;

namespace Game.Gameplay.States
{
    public class LevelBuildState : IState
    {
        private readonly GameplayStateMachine gameplayStateMachine;
        private readonly ILevelBuilder levelBuilder;
        private readonly IInputService inputService;
        private readonly LevelsModel levelsModel;
        private readonly ILogService logService;

        public LevelBuildState(GameplayStateMachine gameplayStateMachine, ILevelBuilder levelBuilder,
            IInputService inputService, LevelsModel levelsModel, ILogService logService)
        {
            this.gameplayStateMachine = gameplayStateMachine;
            this.levelBuilder = levelBuilder;
            this.inputService = inputService;
            this.levelsModel = levelsModel;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("LevelBuildState Enter", LogCategory.Gameplay);

            inputService.SetInputStatus(false);

            LevelPublicRecord record = levelsModel.GetCurrentLevelRecord();

            levelBuilder.Build(record);

            gameplayStateMachine.Enter<LevelProcessState>();
        }

        public async UniTask Exit()
        {
            logService.Log("LevelBuildState Exit", LogCategory.Gameplay);
        }
    }
}
