using Infrastructure.States;
using MessagePipe;
using Services.AssetProvider;
using Services.CurrencyService;
using Services.HapticService;
using Services.InputService;
using Services.LogService;
using Services.NotificationService;
using Services.PrivateContainerProvider;
using Services.PublicContainerProvider;
using Services.SceneProvider;
using Services.TutorialService;
using Services.WindowsService;
using Signals;
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
        [SerializeField] private WindowService windowService;
        [SerializeField] private LoadingCurtain loadingCurtain;
        [SerializeField] private CoroutineRunner coroutineRunner;

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Infrastructure ---
            builder.RegisterEntryPoint<GameBootstrapper>();
            
            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(windowService, Lifetime.Singleton).As<IWindowService>();
            builder.RegisterComponentInNewPrefab(loadingCurtain, Lifetime.Singleton).As<ILoadingCurtain>();
            builder.RegisterComponentInNewPrefab(coroutineRunner, Lifetime.Singleton).As<ICoroutineRunner>();
            
            // --- Game states ---
            builder.Register<GameBootstrapState>(Lifetime.Singleton);
            builder.Register<GameLoadingState>(Lifetime.Singleton);
            builder.Register<GameLobbyState>(Lifetime.Singleton);
            builder.Register<GameplayState>(Lifetime.Singleton);

            // --- Services ---
            builder.Register<PrivateContainerProvider>(Lifetime.Singleton).As<IPrivateContainerProvider>();
            builder.Register<PublicContainerProvider>(Lifetime.Singleton).As<IPublicContainerProvider>();
            builder.Register<TutorialService>(Lifetime.Singleton).As<ITutorialService>();
            builder.Register<CurrencyService>(Lifetime.Singleton).As<ICurrencyService>();
            builder.Register<AssetsProvider>(Lifetime.Singleton).As<IAssetsProvider>();
            builder.Register<SceneProvider>(Lifetime.Singleton).As<ISceneProvider>();
            builder.Register<HapticService>(Lifetime.Singleton).As<IHapticService>();
            builder.Register<LogService>(Lifetime.Singleton).As<ILogService>();
            builder.Register<Factory>(Lifetime.Singleton).As<IFactory>();
            
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            builder.Register<MobileInputService>(Lifetime.Singleton).As<IInputService>().As<ITickable>();
#else
            builder.Register<StandaloneInputService>(Lifetime.Singleton).As<IInputService>().As<ITickable>();
#endif
            
#if UNITY_ANDROID
            builder.Register<NotificationAndroidService>(Lifetime.Singleton).As<INotificationService>();
#elif UNITY_IOS
            builder.Register<NotificationIOSService>(Lifetime.Singleton).As<INotificationService>();
#else
            builder.Register<NullNotificationService>(Lifetime.Singleton).As<INotificationService>();
#endif
            
            // --- Signals ---
            MessagePipeOptions options = builder.RegisterMessagePipe();
            
            builder.RegisterMessageBroker<UpdateCurrencySignal>(options);

            DontDestroyOnLoad(this);
        }
    }
}
