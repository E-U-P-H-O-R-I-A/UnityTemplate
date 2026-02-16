using Cysharp.Threading.Tasks;
using Services.LogService;
using Services.Provider.Public;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPublicModelProvider publicModelProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicModelProvider publicModelProvider)
        {
            this.publicModelProvider = publicModelProvider;
            this.gameStateMachine = gameStateMachine;
            this.logService = logService;
        }
        
        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter");
            
            publicModelProvider.Init();
            
            gameStateMachine.Enter<GameplayState>().Forget();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit");
        }
    }
}