using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Data.Scheme.Public;
using Services.LogService;
using Services.SceneProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;

        public GameplayState(ILogService logService, ISceneProvider sceneProvider, ILoadingCurtain loadingCurtain)
        {
            this.loadingCurtain = loadingCurtain;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GamePlayState Enter", LogCategory.Infrastructure);
            
            var loadSceneTask = sceneProvider.Load(AssetsPath.GAMEPLAY_SCENE);
            
            await loadingCurtain.AnimatePhase(loadSceneTask, 0.90f);
            
            await loadingCurtain.Finish();

            loadingCurtain.Hide();
        }
        
        public async UniTask Exit()
        {
            logService.Log("GamePlayState Exit", LogCategory.Infrastructure);
        }
    }
}
