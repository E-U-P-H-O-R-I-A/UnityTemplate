using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Data.Scheme.Public;
using Services.AssetProvider;
using Services.CurrencyService;
using Services.HapticService;
using Services.LogService;
using Services.NotificationService;
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
        private readonly INotificationService notificationService;
        private readonly IPublicModelProvider publicModelProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ITutorialService tutorialService;
        private readonly ICurrencyService currencyService;
        private readonly IAssetsProvider assetsProvider;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IWindowService windowService;
        private readonly IHapticService hapticService;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicModelProvider publicModelProvider,
            IPrivateModelProvider privateModelProvider, ILoadingCurtain loadingCurtain, ICurrencyService currencyService, 
            IWindowService windowService, ITutorialService tutorialService, INotificationService notificationService,
            IHapticService hapticService, IAssetsProvider assetsProvider)
        {
            this.privateModelProvider = privateModelProvider;
            this.notificationService = notificationService;
            this.publicModelProvider = publicModelProvider;
            this.gameStateMachine = gameStateMachine;
            this.currencyService = currencyService;
            this.tutorialService = tutorialService;
            this.assetsProvider = assetsProvider;
            this.loadingCurtain = loadingCurtain;
            this.windowService = windowService;
            this.hapticService = hapticService;
            this.logService = logService;
        }
        
        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter", LogCategory.Infrastructure);
            
            loadingCurtain.Show();
            
            var assetProviderTask = assetsProvider.Initialize();
            await loadingCurtain.AnimatePhase(assetProviderTask, 0.20f);
            
            var publicDataTask = publicModelProvider.Initialize();
            await loadingCurtain.AnimatePhase(publicDataTask, 0.50f);
            
            var privateDataTask = privateModelProvider.Initizele();
            await loadingCurtain.AnimatePhase(privateDataTask, 0.70f);

            notificationService.Initialize();
            tutorialService.Initialize();
            currencyService.Initialize();
            hapticService.Initialize();
            windowService.Initialize();

            gameStateMachine.Enter<GameLobbyState>();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit", LogCategory.Infrastructure);
        }
    }
}
