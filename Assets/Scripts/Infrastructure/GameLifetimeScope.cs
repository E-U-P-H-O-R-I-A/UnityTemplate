using Infrastructure.States;
using Services.AssetProvider;
using Services.CurrencyService;
using Services.LogService;
using Services.PrivateModelProvider;
using Services.PublicModelProvider;
using Services.SceneProvider;
using UnityEngine;
using Utility.CoroutineRunner;
using Utility.Factory;
using Utility.LoadingCurtain;
using VContainer; 
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Space]
        [SerializeField] private LoadingCurtain loadingCurtain;
        
        protected override void Configure(IContainerBuilder builder)
        {
            // --- Infrastructure ---
            builder.RegisterEntryPoint<GameBootstrapper>();
            
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.RegisterInstance(loadingCurtain).As<ILoadingCurtain>();
            builder.RegisterComponentOnNewGameObject<CoroutineRunner>(Lifetime.Singleton).As<ICoroutineRunner>();
            
            // --- Services ---
            builder.Register<PrivateModelProvider>(Lifetime.Singleton).As<IPrivateModelProvider>();
            builder.Register<PublicModelProvider>(Lifetime.Singleton).As<IPublicModelProvider>();
            builder.Register<CurrencyService>(Lifetime.Singleton).As<ICurrencyService>();
            builder.Register<AssetsProvider>(Lifetime.Singleton).As<IAssetsProvider>();
            builder.Register<SceneProvider>(Lifetime.Singleton).As<ISceneProvider>();
            builder.Register<LogService>(Lifetime.Singleton).As<ILogService>();
            builder.Register<Factory>(Lifetime.Singleton).As<IFactory>();

            // --- Game states ---
            builder.Register<GameBootstrapState>(Lifetime.Singleton);
            builder.Register<GameLoadingState>(Lifetime.Singleton);
            builder.Register<GameplayState>(Lifetime.Singleton);
            
            DontDestroyOnLoad(this);
        }
    }
}