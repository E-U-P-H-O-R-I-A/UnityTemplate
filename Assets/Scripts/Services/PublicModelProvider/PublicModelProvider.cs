using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Services.AssetProvider;
using Services.LogService;
using UnityEngine;

namespace Services.PublicModelProvider
{
    public class PublicModelProvider : IPublicModelProvider
    {
        private readonly Dictionary<Type, IPublicModel> models = new();

        private readonly IAssetsProvider assetsProvider;
        private readonly ILogService logService;

        public PublicModelProvider(IAssetsProvider assetsProvider, ILogService logService)
        {
            this.assetsProvider = assetsProvider;
            this.logService = logService;
        }

        public async UniTask Initialize(CancellationToken ct = default)
        {
            models.Clear();

            var keys = await assetsProvider.GetAssetsListByLabel<ScriptableObject>(AssetsLabels.DATA);
            ct.ThrowIfCancellationRequested();

            var loaded = await assetsProvider.LoadAll<ScriptableObject>(keys);
            ct.ThrowIfCancellationRequested();

            foreach (var asset in loaded)
            {
                if (asset is IPublicModel model)
                    models[asset.GetType()] = model;
            }

            foreach (var t in FindPublicModelTypes().Where(t => !models.ContainsKey(t)))
            {
                logService.LogWarning(
                    $"[PublicModelProvider] No asset found for model {t.FullName} under the {AssetsLabels.DATA} label",
                    LogCategory.PublicModel);
            }
        }

        public TModel GetModel<TModel>() where TModel : IPublicModel
        {
            if (models.TryGetValue(typeof(TModel), out var temp) && temp is TModel typed)
                return typed;

            logService.LogError($"[PublicModelProvider] Model {typeof(TModel).FullName} is not loaded", LogCategory.PublicModel);

            return default;
        }

        #region Helpers

        private static List<Type> FindPublicModelTypes()
        {
            var assembly = typeof(IPublicModel).Assembly;

            return assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(IPublicModel).IsAssignableFrom(t))
                .ToList();
        }

        #endregion
    }
}
