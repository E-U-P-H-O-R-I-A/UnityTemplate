using Infrastructure.States;
using MessagePipe;
using Services;
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
using Services.WindowsService.Factory;
using Services.WindowsService.Windows;
using Signals;
using UnityEngine;
using UnityEngine.Serialization;
using Utility.CoroutineRunner;
using Utility.Factory;
using Utility.LoadingCurtain;
using Utility.StateMachine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameLifetimeScope : LifetimeScope
    {
        [Space]
        [SerializeField] private WindowsRootView windowsRoot;
        [FormerlySerializedAs("loadingCurtain")]
        [SerializeField] private LoadingCurtainView loadingCurtainView;
        [SerializeField] private CoroutineRunner coroutineRunner;

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Infrastructure ---
            builder.RegisterEntryPoint<GameBootstrapper>();

            builder.Register<GameStateMachine>(Lifetime.Singleton).AsSelf().As<IStateMachine>();
            builder.RegisterComponentInNewPrefab(loadingCurtainView, Lifetime.Singleton).AsSelf();
            builder.Register<LoadingCurtainController>(Lifetime.Singleton).As<ILoadingCurtainController>();
            builder.RegisterComponentInNewPrefab(coroutineRunner, Lifetime.Singleton).As<ICoroutineRunner>();
            builder.RegisterComponentInNewPrefab(windowsRoot, Lifetime.Singleton);

            // --- Game states ---
            builder.Register<GameBootstrapState>(Lifetime.Singleton).AsSelf().As<IState>();
            builder.Register<GameLoadingState>(Lifetime.Singleton).AsSelf().As<IState>();
            builder.Register<GameLobbyState>(Lifetime.Singleton).AsSelf().As<IState>();
            builder.Register<GameplayState>(Lifetime.Singleton).AsSelf().As<IState>();

            // --- Data & assets ---
            builder.Register<LogService>(Lifetime.Singleton).As<ILogService>();
            builder.Register<AssetsProvider>(Lifetime.Singleton).As<IAssetsProvider>();
            builder.Register<SceneProvider>(Lifetime.Singleton).As<ISceneProvider>();
            builder.Register<PrivateContainerProvider>(Lifetime.Singleton).As<IPrivateContainerProvider>();
            builder.Register<PublicContainerProvider>(Lifetime.Singleton).As<IPublicContainerProvider>();
            builder.Register<Factory>(Lifetime.Singleton).As<IFactory>();

            // --- Services ---
            builder.Register<WindowFactory>(Lifetime.Singleton).As<IWindowFactory>();
            builder.Register<WindowService>(Lifetime.Singleton).As<IWindowService>().As<IInitializableService>();
            builder.Register<TutorialService>(Lifetime.Singleton).As<ITutorialService>().As<IInitializableService>();
            builder.Register<CurrencyService>(Lifetime.Singleton).As<ICurrencyService>().As<IInitializableService>();
            builder.Register<HapticService>(Lifetime.Singleton).As<IHapticService>().As<IInitializableService>();

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
            builder.Register<MobileInputService>(Lifetime.Singleton).As<IInputService>().As<ITickable>().As<IInitializableService>();
#else
            builder.Register<StandaloneInputService>(Lifetime.Singleton).As<IInputService>().As<ITickable>().As<IInitializableService>();
#endif

#if UNITY_ANDROID
            builder.Register<NotificationAndroidService>(Lifetime.Singleton).As<INotificationService>().As<IInitializableService>();
#elif UNITY_IOS
            builder.Register<NotificationIOSService>(Lifetime.Singleton).As<INotificationService>().As<IInitializableService>();
#else
            builder.Register<NullNotificationService>(Lifetime.Singleton).As<INotificationService>().As<IInitializableService>();
#endif

            // --- Signals ---
            MessagePipeOptions options = builder.RegisterMessagePipe();

            builder.RegisterMessageBroker<UpdateCurrencySignal>(options);
            builder.RegisterMessageBroker<LevelFinishedSignal>(options);
        }
    }
}
