using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Services.LogService;
using Services.SceneProvider;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private readonly ILogService logService;

        public GameplayState(ILogService logService)
        {
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GamePlayState Enter");
        }
        
        public async UniTask Exit()
        {
            logService.Log("GamePlayState Exit");
        }
    }
}