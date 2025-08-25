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
        private ILogService _log;

        public SceneProvider(ILogService log) => _log = log;

        public async UniTask Load(string sceneName)
        {
            AsyncOperationHandle<SceneInstance> handler = Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single, false);

            await handler.ToUniTask();
            await handler.Result.ActivateAsync().ToUniTask();
            
            _log.Log($"Loaded scene '{sceneName}'.");
        }
    }
}