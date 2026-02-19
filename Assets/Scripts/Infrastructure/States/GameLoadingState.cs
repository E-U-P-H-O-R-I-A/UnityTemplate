using Cysharp.Threading.Tasks;
using Services.LogService;
using Services.Provider.Public;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicModelProvider publicModelProvider,
            IPrivateModelProvider privateModelProvider)
        {
            this.privateModelProvider = privateModelProvider;
            this.publicModelProvider = publicModelProvider;
            this.gameStateMachine = gameStateMachine;
            this.logService = logService;
        }
        
        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter");

            privateModelProvider.Init();
            publicModelProvider.Init();

            gameStateMachine.Enter<GameplayState>().Forget();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit");
        }
    }
}