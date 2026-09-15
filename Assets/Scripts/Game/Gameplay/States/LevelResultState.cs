using System;
using Cysharp.Threading.Tasks;
using Game.UI.Lobby.Tabs.Levels;
using Infrastructure;
using Infrastructure.States;
using Services.LevelBuilder;
using Services.LogService;
using Services.WindowsService;
using Signals;
using Utility.StateMachine;
using WindowType = Data.WindowPublicContainer.Id;

namespace Game.Gameplay.States
{
    public class LevelResultState : IPayloadState<LevelResult>, IDisposable
    {
        private readonly GameplayWindowsSettings windowsSettings;
        private readonly GameStateMachine gameStateMachine;
        private readonly IWindowService windowService;
        private readonly ILevelBuilder levelBuilder;
        private readonly LevelsModel levelsModel;
        private readonly ILogService logService;

        private WindowType resultWindow;
        private bool isWaitingForWindow;

        public LevelResultState(GameStateMachine gameStateMachine, GameplayWindowsSettings windowsSettings,
            IWindowService windowService, ILevelBuilder levelBuilder, LevelsModel levelsModel, ILogService logService)
        {
            this.gameStateMachine = gameStateMachine;
            this.windowsSettings = windowsSettings;
            this.windowService = windowService;
            this.levelBuilder = levelBuilder;
            this.levelsModel = levelsModel;
            this.logService = logService;
        }

        public async UniTask Enter(LevelResult result)
        {
            logService.Log($"LevelResultState Enter with result {result}", LogCategory.Gameplay);

            if (result == LevelResult.Win)
                levelsModel.CompleteLevel();

            resultWindow = windowsSettings.GetResultWindow(result);
            isWaitingForWindow = true;

            windowService.WindowClosed += OnWindowClosed;
            windowService.OpenWindow(resultWindow);
        }

        public async UniTask Exit()
        {
            logService.Log("LevelResultState Exit", LogCategory.Gameplay);

            Unsubscribe();
        }

        public void Dispose() =>
            Unsubscribe();

        private void OnWindowClosed(WindowType type)
        {
            if (!isWaitingForWindow || type != resultWindow)
                return;

            Unsubscribe();

            levelBuilder.Clear();

            gameStateMachine.Enter<GameLobbyState>();
        }

        private void Unsubscribe()
        {
            if (!isWaitingForWindow)
                return;

            isWaitingForWindow = false;

            windowService.WindowClosed -= OnWindowClosed;
        }
    }
}
