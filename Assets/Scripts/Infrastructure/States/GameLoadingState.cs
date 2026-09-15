using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using Services.CurrencyService;
using Services.HapticService;
using Services.InputService;
using Services.LogService;
using Services.NotificationService;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;
using Services.SceneProvider;
using Services.TutorialService;
using Services.WindowsService;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPrivateContainerProvider privateContainerProvider;
        private readonly INotificationService notificationService;
        private readonly IPublicContainerProvider publicContainerProvider;
        private readonly GameStateMachine gameStateMachine;
        private readonly ITutorialService tutorialService;
        private readonly ICurrencyService currencyService;
        private readonly IAssetsProvider assetsProvider;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly IWindowService windowService;
        private readonly IHapticService hapticService;
        private readonly IInputService inputService;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, IPublicContainerProvider publicContainerProvider,
            IPrivateContainerProvider privateContainerProvider, ILoadingCurtain loadingCurtain, ICurrencyService currencyService, 
            IWindowService windowService, ITutorialService tutorialService, INotificationService notificationService,
            IHapticService hapticService, IAssetsProvider assetsProvider, IInputService inputService)
        {
            this.inputService = inputService;
            this.privateContainerProvider = privateContainerProvider;
            this.notificationService = notificationService;
            this.publicContainerProvider = publicContainerProvider;
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
            
            var publicDataTask = publicContainerProvider.Initialize();
            await loadingCurtain.AnimatePhase(publicDataTask, 0.50f);
            
            var privateDataTask = privateContainerProvider.Initialize();
            await loadingCurtain.AnimatePhase(privateDataTask, 0.70f);

            notificationService.Initialize();
            tutorialService.Initialize();
            currencyService.Initialize();
            hapticService.Initialize();
            windowService.Initialize();
            inputService.Initialize();

            gameStateMachine.Enter<GameLobbyState>();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit", LogCategory.Infrastructure);
        }
    }
}
