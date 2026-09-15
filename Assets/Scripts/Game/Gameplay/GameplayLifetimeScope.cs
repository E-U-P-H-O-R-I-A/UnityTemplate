using Game.Gameplay.States;
using Game.UI.Lobby.Tabs.Levels;
using Infrastructure;
using Services.LevelBuilder;
using UnityEngine;
using Utility.StateMachine;
using VContainer;
using VContainer.Unity;

namespace Game.Gameplay
{
    public class GameplayLifetimeScope : SceneLifetimeScope
    {
        [Space]
        [SerializeField] private GameplayWindowsSettings windowsSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            // --- Settings ---
            builder.RegisterInstance(windowsSettings);

            // --- Services ---
            builder.Register<LevelBuilder>(Lifetime.Scoped).As<ILevelBuilder>();

            // --- Models ---
            builder.Register<LevelsModel>(Lifetime.Scoped);

            // --- State machine ---
            builder.Register<GameplayStateMachine>(Lifetime.Scoped).AsSelf();

            builder.Register<LevelBuildState>(Lifetime.Scoped).AsSelf().As<IExitableState>();
            builder.Register<LevelProcessState>(Lifetime.Scoped).AsSelf().As<IExitableState>();
            builder.Register<LevelResultState>(Lifetime.Scoped).AsSelf().As<IExitableState>();

            builder.RegisterEntryPoint<GameplayEntryPoint>();
        }
    }
}
