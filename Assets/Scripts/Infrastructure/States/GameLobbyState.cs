using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using Game.UI.Lobby;
using Services.LogService;
using Services.SceneProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.States
{
    public class GameLobbyState : IState
    {
        private readonly ILoadingCurtain loadingCurtain;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;
        
        private Lobby lobby;

        public GameLobbyState(ILogService logService, ILoadingCurtain loadingCurtain, ISceneProvider sceneProvider)
        {
            this.loadingCurtain = loadingCurtain;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GameLobbyState Enter");
            
            var loadSceneTask = sceneProvider.Load(AssetsPath.LOBBY_SCENE);
            
            await loadingCurtain.AnimatePhase(loadSceneTask, 0.90f);
            
            ResolveLobby();
            
            lobby.Initialize();
            
            await loadingCurtain.Finish();

            loadingCurtain.Hide();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLobbyState Exit");
            
            loadingCurtain.Show();
        }

        private void ResolveLobby()
        {
            var lobbyScope = LifetimeScope.Find<LobbyLifeTimeScope>();
            
            lobby = lobbyScope.Container.Resolve<Lobby>();
        }
    }
}
