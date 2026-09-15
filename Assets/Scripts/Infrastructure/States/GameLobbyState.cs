using Cysharp.Threading.Tasks;
using Services.AssetProvider;
using Services.LogService;
using Services.SceneProvider;
using Utility.LoadingCurtain;
using Utility.StateMachine;

namespace Infrastructure.States
{
    public class GameLobbyState : IState
    {
        private readonly ILoadingCurtainController loadingCurtain;
        private readonly ISceneProvider sceneProvider;
        private readonly ILogService logService;

        public GameLobbyState(ILogService logService, ILoadingCurtainController loadingCurtain, ISceneProvider sceneProvider)
        {
            this.loadingCurtain = loadingCurtain;
            this.sceneProvider = sceneProvider;
            this.logService = logService;
        }

        public async UniTask Enter()
        {
            logService.Log("GameLobbyState Enter", LogCategory.Infrastructure);

            await loadingCurtain.Show();

            await loadingCurtain.AnimatePhase(sceneProvider.Load(AssetsPath.LOBBY_SCENE), 0.90f);
            await loadingCurtain.Finish();

            await loadingCurtain.Hide();
        }

        public async UniTask Exit()
        {
            logService.Log("GameLobbyState Exit", LogCategory.Infrastructure);
        }
    }
}
