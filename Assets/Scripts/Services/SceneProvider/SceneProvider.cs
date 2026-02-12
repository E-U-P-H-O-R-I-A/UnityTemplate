using Cysharp.Threading.Tasks;
using Services.LogService;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace Services.SceneProvider
{
    public class SceneProvider : ISceneProvider
    {
        private ILogService logService;

        public SceneProvider(ILogService logService) => this.logService = logService;

        public async UniTask Load(string sceneName)
        {
            AsyncOperationHandle<SceneInstance> handler = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);

            await handler.ToUniTask();
            await handler.Result.ActivateAsync().ToUniTask();
            
            logService.Log($"Loaded scene '{sceneName}'.");
        }
    }
}