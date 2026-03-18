using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Services.CurrencyService;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using Services.SceneProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ICurrencyService currencyService;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicModelProvider publicModelProvider,
            IPrivateModelProvider privateModelProvider, ILoadingCurtain loadingCurtain, ISceneProvider sceneProvider, 
            ICurrencyService currencyService)
        {
            this.currencyService = currencyService;
            this.privateModelProvider = privateModelProvider;
            this.publicModelProvider = publicModelProvider;
            this.gameStateMachine = gameStateMachine;
            this.loadingCurtain = loadingCurtain;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }
        
        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter");
            
            loadingCurtain.Show();
            
            var publicDataTask = publicModelProvider.Init();
            await loadingCurtain.AnimatePhase(publicDataTask, 0.20f);

            var privateDataTask = privateModelProvider.Init();
            await loadingCurtain.AnimatePhase(privateDataTask, 0.70f);
            
            currencyService.Init();
            
            var loadSceneTask = sceneProvider.Load(AssetsPath.MAIN_SCENE);
            await loadingCurtain.AnimatePhase(loadSceneTask, 0.90f);
            
            await loadingCurtain.Finish();

            loadingCurtain.Hide();

            gameStateMachine.Enter<GameplayState>();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit");
        }
    }
}