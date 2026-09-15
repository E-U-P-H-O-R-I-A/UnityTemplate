using System;
using Cysharp.Threading.Tasks;
using MessagePipe;
using Services.InputService;
using Services.LogService;
using Signals;
using Utility.StateMachine;

namespace Game.Gameplay.States
{
    public class LevelProcessState : IState, IDisposable
    {
        private readonly ISubscriber<LevelFinishedSignal> levelFinishedSubscriber;
        private readonly GameplayStateMachine gameplayStateMachine;
        private readonly IInputService inputService;
        private readonly ILogService logService;

        private IDisposable subscription;

        public LevelProcessState(GameplayStateMachine gameplayStateMachine, ISubscriber<LevelFinishedSignal> levelFinishedSubscriber,
            IInputService inputService, ILogService logService)
        {
            this.levelFinishedSubscriber = levelFinishedSubscriber;
            this.gameplayStateMachine = gameplayStateMachine;
            this.inputService = inputService;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("LevelProcessState Enter", LogCategory.Gameplay);

            subscription = levelFinishedSubscriber.Subscribe(OnLevelFinished);

            inputService.SetInputStatus(true);
        }

        public async UniTask Exit()
        {
            logService.Log("LevelProcessState Exit", LogCategory.Gameplay);

            inputService.SetInputStatus(false);

            Unsubscribe();
        }

        public void Dispose() =>
            Unsubscribe();

        private void OnLevelFinished(LevelFinishedSignal signal)
        {
            Unsubscribe();

            gameplayStateMachine.Enter<LevelResultState, LevelResult>(signal.result);
        }

        private void Unsubscribe()
        {
            subscription?.Dispose();
            subscription = null;
        }
    }
}
