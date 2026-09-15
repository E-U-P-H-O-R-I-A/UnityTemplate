using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Data;
using Services.AssetProvider;
using Services.LogService;
using UnityEngine;

namespace Services.PublicContainerProvider
{
    public class PublicContainerProvider : IPublicContainerProvider
    {
        private readonly Dictionary<Type, IPublicContainer> containers = new();

        private readonly IAssetsProvider assetsProvider;
        private readonly ILogService logService;

        public PublicContainerProvider(IAssetsProvider assetsProvider, ILogService logService)
        {
            this.assetsProvider = assetsProvider;
            this.logService = logService;
        }

        public async UniTask Initialize(CancellationToken ct = default)
        {
            containers.Clear();

            var keys = await assetsProvider.GetAssetsListByLabel<ScriptableObject>(AssetsLabels.DATA);
            ct.ThrowIfCancellationRequested();

            var loaded = await assetsProvider.LoadAll<ScriptableObject>(keys);
            ct.ThrowIfCancellationRequested();

            foreach (var asset in loaded)
            {
                if (asset is IPublicContainer container)
                    containers[asset.GetType()] = container;
            }

            foreach (var t in FindPublicContainerTypes().Where(t => !containers.ContainsKey(t)))
            {
                logService.LogWarning(
                    $"[PublicContainerProvider] No asset found for container {t.FullName} under the {AssetsLabels.DATA} label",
                    LogCategory.PublicContainer);
            }
        }

        public TContainer GetContainer<TContainer>() where TContainer : IPublicContainer
        {
            if (containers.TryGetValue(typeof(TContainer), out var temp) && temp is TContainer typed)
                return typed;

            throw new InvalidOperationException($"[PublicContainerProvider] Container {typeof(TContainer).FullName} is not loaded");
        }

        #region Helpers

        private static List<Type> FindPublicContainerTypes()
        {
            var assembly = typeof(IPublicContainer).Assembly;

            return assembly
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(IPublicContainer).IsAssignableFrom(t))
                .ToList();
        }

        #endregion
    }
}
