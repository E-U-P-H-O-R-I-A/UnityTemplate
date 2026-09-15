using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Services.LogService;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Services.AssetProvider
{
    public class AssetsProvider : IAssetsProvider
    {
        private readonly Dictionary<(string key, Type type), AsyncOperationHandle> assetRequests = new();
        private readonly ILogService logService;

        public AssetsProvider(ILogService logService)
        {
            this.logService = logService;
        }

        public async UniTask Initialize() =>
            await Addressables.InitializeAsync().ToUniTask();

        public async UniTask<TAsset> Load<TAsset>(string key) where TAsset : class
        {
            try
            {
                var handle = GetOrCreateHandle<TAsset>(key);

                await handle.ToUniTask();

                return handle.Result as TAsset;
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to load asset with key {key}, error : {e}", LogCategory.Service);

                return null;
            }
        }

        public async UniTask<TAsset> LoadPrefab<TAsset>(string key) where TAsset : class
        {
            var prefab = await Load<GameObject>(key);

            return prefab == null ? null : prefab.GetComponent<TAsset>();
        }

        public async UniTask<TAsset> Load<TAsset>(AssetReference assetReference) where TAsset : class =>
            await Load<TAsset>(assetReference.AssetGUID);

        public async UniTask<List<string>> GetAssetsListByLabel<TAsset>(string label) =>
            await GetAssetsListByLabel(label, typeof(TAsset));

        public async UniTask<List<string>> GetAssetsListByLabel(string label, Type type = null)
        {
            try
            {
                var operationHandle = Addressables.LoadResourceLocationsAsync(label, type);
                var locations = await operationHandle.ToUniTask();

                var assetKeys = new List<string>(locations.Count);

                foreach (IResourceLocation location in locations)
                    assetKeys.Add(location.PrimaryKey);

                Addressables.Release(operationHandle);

                return assetKeys;
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to get assets list by label {label}, error : {e}", LogCategory.Service);

                return new List<string>();
            }
        }

        public async UniTask<TAsset[]> LoadAll<TAsset>(List<string> keys) where TAsset : class
        {
            var tasks = new List<UniTask<TAsset>>(keys.Count);

            foreach (string key in keys)
                tasks.Add(Load<TAsset>(key));

            return await UniTask.WhenAll(tasks);
        }

        public async UniTask WarmupAssetsByLabel(string label)
        {
            var assetsList = await GetAssetsListByLabel(label);
            await LoadAll<object>(assetsList);
        }

        public async UniTask ReleaseAssetsByLabel(string label)
        {
            var assetsList = await GetAssetsListByLabel(label);
            var keysToRelease = new HashSet<string>(assetsList);

            foreach (var cacheKey in assetRequests.Keys.Where(cacheKey => keysToRelease.Contains(cacheKey.key)).ToList())
            {
                Addressables.Release(assetRequests[cacheKey]);
                assetRequests.Remove(cacheKey);
            }
        }

        public void Cleanup()
        {
            foreach (var assetRequest in assetRequests)
                Addressables.Release(assetRequest.Value);

            assetRequests.Clear();
        }

        private AsyncOperationHandle GetOrCreateHandle<TAsset>(string key) where TAsset : class
        {
            var cacheKey = (key, typeof(TAsset));

            if (assetRequests.TryGetValue(cacheKey, out var handle))
                return handle;

            handle = Addressables.LoadAssetAsync<TAsset>(key);
            assetRequests.Add(cacheKey, handle);

            return handle;
        }
    }
}
