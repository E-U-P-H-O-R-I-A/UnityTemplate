using Cysharp.Threading.Tasks;
using Services.LogService;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService)
        {
            this.gameStateMachine = gameStateMachine;
            this.logService = logService;
        }
        
        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter");
            
            gameStateMachine.Enter<GameplayState>().Forget();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit");
        }
    }
}