using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Services.CurrencyService;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using Services.SceneProvider;
using Services.TutorialService;
using Services.WindowsService;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPrivateModelProvider privateModelProvider;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ITutorialService tutorialService;
        private readonly ICurrencyService currencyService;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IWindowService windowService;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicModelProvider publicModelProvider,
            IPrivateModelProvider privateModelProvider, ILoadingCurtain loadingCurtain, ICurrencyService currencyService, 
            IWindowService windowService, ITutorialService tutorialService)
        {
            this.privateModelProvider = privateModelProvider;
            this.publicModelProvider = publicModelProvider;
            this.gameStateMachine = gameStateMachine;
            this.currencyService = currencyService;
            this.tutorialService = tutorialService;
            this.loadingCurtain = loadingCurtain;
            this.windowService = windowService;
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

            tutorialService.Initialize();
            currencyService.Initialize();
            windowService.Initialize();

            gameStateMachine.Enter<GameLobbyState>();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit");
        }
    }
}
