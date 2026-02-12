using Infrastructure.States;
using Services.AssetProvider;
using Services.LogService;
using Services.SceneProvider;
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
            // --- Infrastructure ---
            builder.RegisterEntryPoint<GameBootstrapper>();
            
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.RegisterComponentOnNewGameObject<CoroutineRunner>(Lifetime.Singleton).As<ICoroutineRunner>();
            
            // --- Services ---
            builder.Register<AssetsProvider>(Lifetime.Singleton).As<IAssetsProvider>();
            builder.Register<SceneProvider>(Lifetime.Singleton).As<ISceneProvider>();
            builder.Register<LogService>(Lifetime.Singleton).As<ILogService>();
            builder.Register<Factory>(Lifetime.Singleton).As<IFactory>();
            
            // --- Game states ---
            builder.Register<GameBootstrapState>(Lifetime.Singleton);
            builder.Register<GameLoadingState>(Lifetime.Singleton);
            builder.Register<GameplayState>(Lifetime.Singleton);
        }
    }
}