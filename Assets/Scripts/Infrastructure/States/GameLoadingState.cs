using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Services;
using Services.AssetProvider;
using Services.LogService;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLoadingState : IState
    {
        private readonly IPrivateContainerProvider privateContainerProvider;
        private readonly IPublicContainerProvider publicContainerProvider;
        private readonly IEnumerable<IInitializableService> services;
        private readonly GameStateMachine gameStateMachine;
        private readonly IAssetsProvider assetsProvider;
        private readonly ILoadingCurtain loadingCurtain;
        private readonly ILogService logService;

        public GameLoadingState(GameStateMachine gameStateMachine, ILogService logService, ILoadingCurtain loadingCurtain,
            IAssetsProvider assetsProvider, IPublicContainerProvider publicContainerProvider,
            IPrivateContainerProvider privateContainerProvider, IEnumerable<IInitializableService> services)
        {
            this.privateContainerProvider = privateContainerProvider;
            this.publicContainerProvider = publicContainerProvider;
            this.gameStateMachine = gameStateMachine;
            this.assetsProvider = assetsProvider;
            this.loadingCurtain = loadingCurtain;
            this.logService = logService;
            this.services = services;
        }

        public async UniTask Enter()
        {
            logService.Log("GameLoadingState Enter", LogCategory.Infrastructure);

            loadingCurtain.Show();

            await loadingCurtain.AnimatePhase(assetsProvider.Initialize(), 0.20f);
            await loadingCurtain.AnimatePhase(publicContainerProvider.Initialize(), 0.50f);
            await loadingCurtain.AnimatePhase(privateContainerProvider.Initialize(), 0.70f);

            foreach (var service in services)
                service.Initialize();

            gameStateMachine.Enter<GameLobbyState>();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLoadingState Exit", LogCategory.Infrastructure);
        }
    }
}
