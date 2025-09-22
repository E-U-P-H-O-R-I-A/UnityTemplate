using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Services.LogService;
using Services.SceneProvider;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ISceneProvider sceneProvider, 
            ILogService logService)
        {
            this.gameStateMachine = gameStateMachine;
            this.sceneProvider = sceneProvider;
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