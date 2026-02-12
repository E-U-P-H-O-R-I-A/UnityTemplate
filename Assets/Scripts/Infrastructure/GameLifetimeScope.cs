using CodeBase.Infrastructure.AssetManagement;
using Services.AssetProvider;
using Services.LogService;
using Services.SceneProvider;
using UnityEngine;
using Utility.CoroutineRunner;
using Utility.Factory;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // --- Services ---
            builder.Register<AssetsProvider>(Lifetime.Singleton).As<IAssetsProvider>();
            builder.Register<SceneProvider>(Lifetime.Singleton).As<ISceneProvider>();
            builder.Register<LogService>(Lifetime.Singleton).As<ILogService>();
            builder.Register<Factory>(Lifetime.Singleton).As<IFactory>();

            // --- Infrastructure ---
            builder.Register<GameStateMachine>(Lifetime.Singleton);

            // Prefab from Resources
            var bootstrapperPrefab = Resources.Load<GameObject>(AssetsPath.GAMEBOOTSTRAPPER);
            var runnerPrefab = Resources.Load<GameObject>(AssetsPath.COROUTINE_RUNNER);

            builder.RegisterComponentInNewPrefab(
                bootstrapperPrefab.GetComponent<GameBootstrapper>(),
                Lifetime.Singleton)
                .As<GameBootstrapper>();

            builder.RegisterComponentInNewPrefab(
                    runnerPrefab.GetComponent<CoroutineRunner>(),
                    Lifetime.Singleton)
                .As<ICoroutineRunner>();
            
            builder.RegisterBuildCallback(container =>
            {
                container.Resolve<GameStateMachine>();
                container.Resolve<GameBootstrapper>();
                container.Resolve<ICoroutineRunner>();
            });
        }
    }
}