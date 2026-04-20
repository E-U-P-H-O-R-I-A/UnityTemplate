using System;
using System.Collections.Generic;
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
        private readonly Dictionary<string, AsyncOperationHandle> assetRequests = new ();
        private readonly ILogService logService;

        public AssetsProvider(ILogService logService)
        {
            this.logService = logService;
        }

        public async UniTask InitializeAsync() => 
            await Addressables.InitializeAsync().ToUniTask();

        public async UniTask<TAsset> Load<TAsset>(string key) where TAsset : class
        {
            AsyncOperationHandle handle;

            try
            {
                if (!assetRequests.TryGetValue(key, out handle))
                {
                    handle = Addressables.LoadAssetAsync<TAsset>(key);
                    assetRequests.Add(key, handle);
                }

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
            AsyncOperationHandle handle;

            try
            {
                if (!assetRequests.TryGetValue(key, out handle))
                {
                    handle = Addressables.LoadAssetAsync<GameObject>(key);
                    assetRequests.Add(key, handle);
                }

                await handle.ToUniTask();
                var prefab = handle.Result as GameObject;
                
                return prefab.GetComponent<TAsset>();
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to load asset with key {key}, error : {e}", LogCategory.Service);
                
                return null;
            }
        }

        public async UniTask<TAsset> Load<TAsset>(AssetReference assetReference) where TAsset : class
        {
            try
            {
                return await Load<TAsset>(assetReference.AssetGUID);
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to load asset with reference {assetReference}, error : {e}", LogCategory.Service);
                
                return null;
            }
        }

        public async UniTask<List<string>> GetAssetsListByLabel<TAsset>(string label)
        {
            try
            {
                return await GetAssetsListByLabel(label, typeof(TAsset));
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to get assets list by label {label}, error : {e}", LogCategory.Service);
                
                return new List<string>();
            }
        }

        public async UniTask<List<string>> GetAssetsListByLabel(string label, Type type = null)
        {
            try
            {
                var operationHandle = Addressables.LoadResourceLocationsAsync(label, type);
                var locations = await operationHandle.ToUniTask();

                List<string> assetKeys = new List<string>(locations.Count);

                foreach (IResourceLocation location in locations)
                {
                    assetKeys.Add(location.PrimaryKey);
                }

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
            try
            {
                List<UniTask<TAsset>> tasks = new List<UniTask<TAsset>>(keys.Count);

                foreach (string key in keys)
                {
                    tasks.Add(Load<TAsset>(key));
                }

                return await UniTask.WhenAll(tasks);
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to load all assets, error : {e}", LogCategory.Service);
                
                return Array.Empty<TAsset>();
            }
        }

        public async UniTask WarmupAssetsByLabel(string label)
        {
            try
            {
                var assetsList = await GetAssetsListByLabel(label);
                await LoadAll<object>(assetsList);
            }
            catch (Exception e)
            {
                logService.LogError($"Failed to warmup assets by label {label}, error : {e}", LogCategory.Service);
            }
        }

        public async UniTask ReleaseAssetsByLabel(string label)
        {
            var assetsList = await GetAssetsListByLabel(label);
            
            foreach (var assetKey in assetsList)
            {
                if (assetRequests.TryGetValue(assetKey, out var handler))
                {
                    Addressables.Release(handler);
                    assetRequests.Remove(assetKey);
                }
            }
        }

        public void Cleanup()
        {
            foreach (var assetRequest in assetRequests)
            {
                Addressables.Release(assetRequest.Value);
            }

            assetRequests.Clear();
        }
    }
}
