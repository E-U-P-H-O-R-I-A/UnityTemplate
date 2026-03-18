using Infrastructure.States;
using Utility.Factory;
using VContainer.Unity;

namespace Infrastructure
{
    public class GameBootstrapper : IStartable
    {
        private readonly GameStateMachine gameStateMachine;
        private readonly IFactory factory;
        
        public GameBootstrapper(GameStateMachine gameStateMachine, IFactory factory)
        {
            this.gameStateMachine = gameStateMachine;
            this.factory = factory;
        }

        public void Start()
        {
            gameStateMachine.RegisterState(factory.Create<GameBootstrapState>());
            gameStateMachine.RegisterState(factory.Create<GameLoadingState>());
            gameStateMachine.RegisterState(factory.Create<GameplayState>()); 
            
            gameStateMachine.Enter<GameBootstrapState>();
        }
    }
}
