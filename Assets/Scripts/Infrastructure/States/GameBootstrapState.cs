using Cysharp.Threading.Tasks;
using Services.LogService;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameBootstrapState : IState
    {
        private readonly ILogService logService;
        private readonly GameStateMachine gameStateMachine;

        public GameBootstrapState(GameStateMachine gameStateMachine, ILogService logService)
        {
            this.gameStateMachine = gameStateMachine;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GameBootstrapState Enter");
            
            gameStateMachine.Enter<GameLoadingState>();
        }
        
        public async UniTask Exit()
        {
            logService.Log("GameBootstrapState Exit");
        }
    }
}