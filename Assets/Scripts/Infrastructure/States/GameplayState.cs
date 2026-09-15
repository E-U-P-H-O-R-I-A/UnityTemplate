using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using Services.LogService;
using Services.SceneProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameplayState : IState
    {
        private readonly ILoadingCurtainController loadingCurtain;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;

        public GameplayState(ILogService logService, ISceneProvider sceneProvider, ILoadingCurtainController loadingCurtain)
        {
            this.loadingCurtain = loadingCurtain;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GamePlayState Enter", LogCategory.Infrastructure);
            
            await loadingCurtain.Show();
            
            var loadSceneTask = sceneProvider.Load(AssetsPath.GAMEPLAY_SCENE);
            
            await loadingCurtain.AnimatePhase(loadSceneTask, 0.90f);
            
            await loadingCurtain.Finish();

            await loadingCurtain.Hide();
        }
        
        public async UniTask Exit()
        {
            logService.Log("GamePlayState Exit", LogCategory.Infrastructure);
        }
    }
}
